using System;
using System.Collections.Generic;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.ScriptProviders;
using NSubstitute;

namespace DbUp.Specification
{
    public class UpgradeEngineTests
    {

        public class when_marking_scripts_as_read : SpecificationFor<UpgradeEngine>
        {
            private IJournal versionTracker;
            private IScriptProvider scriptProvider;
            private IScriptExecutor scriptExecutor;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScripts().Returns(new List<SqlScript> {new SqlScript("1234", "foo")});
                versionTracker = Substitute.For<IJournal>();
                scriptExecutor = Substitute.For<IScriptExecutor>();

                var config = new UpgradeConfiguration();
                config.ScriptProviders.Add(scriptProvider);
                config.ScriptExecutor = scriptExecutor;
                config.Journal = versionTracker;

                var upgrader = new UpgradeEngine(config);
                return upgrader;
            }

            public override void When()
            {
                Subject.MarkAsExecuted();
            }

            [Then]
            public void the_scripts_are_journalled()
            {
                versionTracker.Received().StoreExecutedScript(Arg.Is<SqlScript>(s => s.Name == "1234"));
            }

            [Then]
            public void the_scripts_are_not_run()
            {
                scriptExecutor.DidNotReceiveWithAnyArgs().Execute(null);
            }
        }

    }
}