using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.Support;
using DbUp.Tests.Common;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;

namespace DbUp.Tests.Engine;

public class UpgradeEngineTests
{
    public class when_upgrading_a_database_with_variable_substitution : SpecificationFor<UpgradeEngine>
    {
        IDbCommand dbCommand;

        public override UpgradeEngine Given()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            dbCommand = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(dbCommand);
            var connectionManager = new TestConnectionManager(dbConnection);

            var builder = new UpgradeEngineBuilder()
                .WithScript(new SqlScript("1234", "create table $var$ (Id int)"))
                .JournalTo(new NullJournal())
                .WithVariable("var", "sub");
            builder.Configure(c => c.ScriptExecutor = new TestScriptExecutor(c, "dbo"));
            builder.Configure(c => c.ConnectionManager = connectionManager);

            var upgrader = builder.Build();
            return upgrader;
        }

        protected override void When()
        {
            Subject.PerformUpgrade();
        }

        [Then]
        public void substitutes_variable()
        {
            dbCommand.CommandText.ShouldBe("create table sub (Id int)");
        }
    }

    public class when_marking_scripts_as_read : SpecificationFor<UpgradeEngine>
    {
        IJournal versionTracker;
        IScriptProvider scriptProvider;
        IScriptExecutor scriptExecutor;

        public override UpgradeEngine Given()
        {
            scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript> {new("1234", "foo")});
            versionTracker = Substitute.For<IJournal>();
            scriptExecutor = Substitute.For<IScriptExecutor>();

            var config = new UpgradeConfiguration {ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())};
            config.ScriptProviders.Add(scriptProvider);
            config.ScriptExecutor = scriptExecutor;
            config.Journal = versionTracker;

            var upgrader = new UpgradeEngine(config);
            return upgrader;
        }

        protected override void When()
        {
            Subject.MarkAsExecuted();
        }

        [Then]
        public void the_scripts_are_journalled()
        {
            versionTracker.Received().StoreExecutedScript(Arg.Is<SqlScript>(s => s.Name == "1234"), Arg.Any<Func<IDbCommand>>());
        }

        [Then]
        public void the_scripts_are_not_run()
        {
            scriptExecutor.DidNotReceiveWithAnyArgs().Execute(null);
        }
    }

    public class when_marking_as_executed_a_script_that_was_not_discovered : SpecificationFor<UpgradeEngine>
    {
        InMemoryJournal journal;
        DatabaseUpgradeResult result;

        public override UpgradeEngine Given()
        {
            var scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript>
            {
                new("Script1.sql", "foo"),
                new("Script2.sql", "bar"),
                new("Script3.sql", "baz")
            });

            journal = new InMemoryJournal(new CaptureLogsLogger());
            journal.AddScriptsAsPreviouslyExecuted(new List<SqlScript> {new("Script1.sql", "foo")});

            var config = new UpgradeConfiguration {ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())};
            config.ScriptProviders.Add(scriptProvider);
            config.ScriptExecutor = Substitute.For<IScriptExecutor>();
            config.Journal = journal;

            return new UpgradeEngine(config);
        }

        protected override void When()
        {
            result = Subject.MarkAsExecuted("ScriptThatWasNotDiscovered.sql");
        }

        [Then]
        public void the_result_is_unsuccessful()
        {
            result.Successful.ShouldBeFalse();
        }

        [Then]
        public void the_error_identifies_the_script_that_was_not_found()
        {
            result.Error.ShouldNotBeNull();
            result.Error.Message.ShouldContain("ScriptThatWasNotDiscovered.sql");
        }

        [Then]
        public void no_scripts_are_reported_as_marked()
        {
            result.Scripts.ShouldBeEmpty();
        }

        [Then]
        public void the_journal_is_left_unchanged()
        {
            journal.GetExecutedScripts().ShouldBe(new[] {"Script1.sql"});
        }
    }

    public class when_marking_as_executed_a_pending_script_by_name : SpecificationFor<UpgradeEngine>
    {
        InMemoryJournal journal;
        DatabaseUpgradeResult result;

        public override UpgradeEngine Given()
        {
            var scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript>
            {
                new("Script1.sql", "foo"),
                new("Script2.sql", "bar"),
                new("Script3.sql", "baz")
            });

            journal = new InMemoryJournal(new CaptureLogsLogger());
            journal.AddScriptsAsPreviouslyExecuted(new List<SqlScript> {new("Script1.sql", "foo")});

            var config = new UpgradeConfiguration {ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())};
            config.ScriptProviders.Add(scriptProvider);
            config.ScriptExecutor = Substitute.For<IScriptExecutor>();
            config.Journal = journal;

            return new UpgradeEngine(config);
        }

        protected override void When()
        {
            result = Subject.MarkAsExecuted("Script2.sql");
        }

        [Then]
        public void the_result_is_successful()
        {
            result.Successful.ShouldBeTrue();
        }

        [Then]
        public void only_the_named_script_is_marked()
        {
            result.Scripts.Select(s => s.Name).ShouldBe(new[] {"Script2.sql"});
        }

        [Then]
        public void only_the_named_script_is_added_to_the_journal()
        {
            journal.GetExecutedScripts().ShouldBe(new[] {"Script1.sql", "Script2.sql"});
        }
    }

    // Preserved pre-existing behaviour, documented here so it is a conscious
    // choice rather than an accident: when the named script has already been
    // executed, it is not part of the scripts-to-execute list the marking loop
    // walks, so the loop's stop condition never matches and every pending
    // script is marked - the "up to and including" semantics applied to a
    // name that is discovered but no longer pending. The fix for undiscovered
    // names deliberately does not change this case.
    public class when_marking_as_executed_a_script_that_was_already_executed : SpecificationFor<UpgradeEngine>
    {
        InMemoryJournal journal;
        DatabaseUpgradeResult result;

        public override UpgradeEngine Given()
        {
            var scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript>
            {
                new("Script1.sql", "foo"),
                new("Script2.sql", "bar"),
                new("Script3.sql", "baz")
            });

            journal = new InMemoryJournal(new CaptureLogsLogger());
            journal.AddScriptsAsPreviouslyExecuted(new List<SqlScript> {new("Script1.sql", "foo")});

            var config = new UpgradeConfiguration {ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())};
            config.ScriptProviders.Add(scriptProvider);
            config.ScriptExecutor = Substitute.For<IScriptExecutor>();
            config.Journal = journal;

            return new UpgradeEngine(config);
        }

        protected override void When()
        {
            result = Subject.MarkAsExecuted("Script1.sql");
        }

        [Then]
        public void the_result_is_successful()
        {
            result.Successful.ShouldBeTrue();
        }

        [Then]
        public void all_pending_scripts_are_marked()
        {
            journal.GetExecutedScripts().ShouldBe(new[] {"Script1.sql", "Script2.sql", "Script3.sql"});
        }
    }

    public class when_querying_discovered_scripts : SpecificationFor<UpgradeEngine>
    {
        IJournal versionTracker;
        IScriptProvider scriptProvider;
        IScriptExecutor scriptExecutor;
        List<string> discoveredScripts;

        public override UpgradeEngine Given()
        {
            scriptProvider = Substitute.For<IScriptProvider>();
            versionTracker = Substitute.For<IJournal>();
            versionTracker.GetExecutedScripts().Returns(new[] {"#1", "#2", "#3"});
            scriptExecutor = Substitute.For<IScriptExecutor>();

            var config = new UpgradeConfiguration {ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())};
            config.ScriptProviders.Add(scriptProvider);
            config.ScriptExecutor = scriptExecutor;
            config.Journal = versionTracker;

            var upgrader = new UpgradeEngine(config);
            return upgrader;
        }

        protected override void When()
        {
            discoveredScripts = Subject.GetExecutedScripts();
        }

        [Then]
        public void discovered_scripts_are_returned()
        {
            discoveredScripts.ShouldBe(new[] {"#1", "#2", "#3"});
        }
    }

    public class when_querying_executed_but_not_discovered_scripts : SpecificationFor<UpgradeEngine>
    {
        IJournal versionTracker;
        IScriptProvider scriptProvider;
        IScriptExecutor scriptExecutor;
        List<string> discoveredScripts;

        public override UpgradeEngine Given()
        {
            scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript> {new("#1", "Content of #1"), new("#3", "Content of #3"),});
            versionTracker = Substitute.For<IJournal>();
            versionTracker.GetExecutedScripts().Returns(new[] {"#1", "#2", "#3"});
            scriptExecutor = Substitute.For<IScriptExecutor>();

            var config = new UpgradeConfiguration {ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())};
            config.ScriptProviders.Add(scriptProvider);
            config.ScriptExecutor = scriptExecutor;
            config.Journal = versionTracker;

            var upgrader = new UpgradeEngine(config);
            return upgrader;
        }

        protected override void When()
        {
            discoveredScripts = Subject.GetExecutedButNotDiscoveredScripts();
        }

        [Then]
        public void discovered_scripts_are_returned()
        {
            discoveredScripts.ShouldBe(new[] {"#2"});
        }
    }
}
