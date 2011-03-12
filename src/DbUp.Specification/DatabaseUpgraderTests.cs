using System;
using System.Collections.Generic;
using DbUp.Execution;
using DbUp.Journal;
using DbUp.ScriptProviders;
using NSubstitute;

namespace DbUp.Specification
{
    public class DatabaseUpgraderTests
    {

        public class when_marking_scripts_as_read : SpecificationFor<DatabaseUpgrader>
        {
            private IJournal _versionTracker;
            private IScriptProvider _scriptProvider;
            private IScriptExecutor _scriptExecutor;

            public override DatabaseUpgrader Given()
            {
                _scriptProvider = Substitute.For<IScriptProvider>();
                _scriptProvider.GetScripts().Returns(new List<SqlScript> {new SqlScript("1234", "foo")});

                _versionTracker = Substitute.For<IJournal>();

                _scriptExecutor = Substitute.For<IScriptExecutor>();

                return new DatabaseUpgrader("connstr", _scriptProvider, _versionTracker, _scriptExecutor);
            }

            public override void When()
            {
                Subject.MarkAsExecuted();
            }

            [Then]
            public void the_scripts_are_journalled()
            {
                _versionTracker.Received().StoreExecutedScript(Arg.Is<SqlScript>(s => s.Name == "1234"));
            }

            [Then]
            public void the_scripts_are_not_run()
            {
                _scriptExecutor.DidNotReceiveWithAnyArgs().Execute(null);
            }
        }

    }
}