using NUnit.Framework;
using DbUp.Specification.Contexts;
using System.Linq;
using NSubstitute;
using System;

namespace DbUp.Specification.Concerning
{
    [TestFixture]
    public class CreatingANewDatabase : GivenANewDatabase
    {
        [Test]
        public void ShouldRunAllScripts()
        {
            var result = DbUpgrader.PerformUpgrade(Log);
            Assert.IsTrue(result.Scripts.All(script => 
                AllScripts.Contains(script)
            ));
        }

        [Test]
        public void ShouldRunAllScriptsInTheOrderProvided()
        {
            var result = DbUpgrader.PerformUpgrade(Log);
            Assert.AreEqual("0001.sql", result.Scripts.ElementAt(0).Name);
            Assert.AreEqual("0004.sql", result.Scripts.ElementAt(1).Name);
            Assert.AreEqual("0002.sql", result.Scripts.ElementAt(2).Name);
        }

        [Test]
        public void ShouldLogAnErrorWhenUpgradeFails()
        {
            var ex = new InvalidOperationException();
            ScriptProvider.GetScripts().Returns(provider => { throw ex; });
            DbUpgrader.PerformUpgrade(Log);
            Log.Received().WriteError("Upgrade failed", ex);
        }

        [Test]
        public void ShouldReturnFailedResult()
        {
            var ex = new InvalidOperationException();
            ScriptProvider.GetScripts().Returns(provider => { throw ex; });
            var result = DbUpgrader.PerformUpgrade(Log);
            
            Assert.That(result.Successful == false);
            Assert.That(result.Scripts.Count() == 0);
            Assert.That(result.Error == ex);
        }

        [Test]
        public void ShouldTrackExecutedScripts()
        {
            DbUpgrader
                .PerformUpgrade(Log)
                .Scripts.ToList()
                .ForEach(script => VersionTracker.Received().StoreExecutedScript(script, Log));
        }
    }
}
