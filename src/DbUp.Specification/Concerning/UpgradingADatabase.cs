using System;
using System.Collections.Generic;
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
			Assert.IsTrue(DbUpgrader.IsUpgradeRequired());
		}
		
		[Test]
		public void ShouldReturnSuccessfulResult ()
		{
			var result = DbUpgrader.PerformUpgrade ();
			Assert.IsTrue (result.Successful);
		}

        [Test]
        public void ShouldExecuteCorrectScript()
        {
            var result = DbUpgrader.PerformUpgrade();
            var expectedScript = AllScripts.Last();
            
            ScriptExecutor.Received().Execute(expectedScript, Arg.Any<IDictionary<string, string>>());
            VersionTracker.Received().StoreExecutedScript(expectedScript);

            Assert.AreEqual(expectedScript, result.Scripts.First());
        }
		
		[Test]
		public void ShouldLogInformation ()
		{
			DbUpgrader.PerformUpgrade ();
			
			Log.Received ().WriteInformation ("Beginning database upgrade");
            Log.Received().WriteInformation("Upgrade successful");
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

