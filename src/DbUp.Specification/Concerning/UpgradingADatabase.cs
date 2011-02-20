using System;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using DbUp.Specification.Contexts;

namespace DbUp.Specification.Concerning
{
    [TestFixture]
	public class UpgradingAnOutOfDateDatabase : GivenAnOutOfDateDatabase
	{
		[Test]
		public void ShouldDetermineIfUpgradeIsRequired ()
		{
			Assert.IsTrue(DbUpgrader.IsUpgradeRequired(Log));
		}
		
		[Test]
		public void ShouldReturnSuccessfulResult ()
		{
			var result = DbUpgrader.PerformUpgrade (Log);
			Assert.IsTrue (result.Successful);
		}

        [Test]
        public void ShouldExecuteCorrectScript()
        {
            var result = DbUpgrader.PerformUpgrade(Log);
            var expectedScript = AllScripts.Last();
            
            ScriptExecutor.Received().Execute(ConnectionString, expectedScript, Log);
            VersionTracker.Received().StoreExecutedScript(expectedScript, Log);

            Assert.AreEqual(expectedScript, result.Scripts.First());
        }
		
		[Test]
		public void ShouldLogInformation ()
		{
			DbUpgrader.PerformUpgrade (Log);
			
			Log.Received ().WriteInformation ("Beginning database upgrade. Connection string is: '{0}'", ConnectionString);
            Log.Received().WriteInformation("Upgrade successful");
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

