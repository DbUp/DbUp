using System;
using System.Linq;
using DbUp.Engine.Transactions;
using DbUp.Tests.Specifications.Contexts;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Specifications
{
    [TestFixture]
    public class CreatingANewDatabase : GivenANewDatabase
    {
        [Test]
        public void ShouldRunAllScripts()
        {
            var result = DbUpgrader.PerformUpgrade();
            Assert.IsTrue(result.Scripts.All(script => 
                AllScripts.Contains(script)
            ));
        }

        [Test]
        public void ShouldRunAllScriptsInTheOrderProvided()
        {
            var result = DbUpgrader.PerformUpgrade();
            Assert.AreEqual("0001.sql", result.Scripts.ElementAt(0).Name);
            Assert.AreEqual("0004.sql", result.Scripts.ElementAt(1).Name);
            Assert.AreEqual("0002.sql", result.Scripts.ElementAt(2).Name);
        }

        [Test]
        public void ShouldLogAnErrorWhenUpgradeFails()
        {
            var ex = new InvalidOperationException();
            ScriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(provider => { throw ex; });
            DbUpgrader.PerformUpgrade();
            Log.Received().WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
        }

        [Test]
        public void ShouldReturnFailedResult()
        {
            var ex = new InvalidOperationException();
            ScriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(provider => { throw ex; });
            var result = DbUpgrader.PerformUpgrade();
            
            Assert.That(result.Successful == false);
            Assert.That(!result.Scripts.Any());
            Assert.That(result.Error == ex);
        }

        [Test]
        public void ShouldTrackExecutedScripts()
        {
            DbUpgrader
                .PerformUpgrade()
                .Scripts.ToList()
                .ForEach(script => VersionTracker.Received().StoreExecutedScript(script));
        }
    }
}
