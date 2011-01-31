using System;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using DbUp.Specification.Contexts;

namespace DbUp.Specification.Concerning
{
	[TestFixture]
	public class UpgradingADatabase : GivenAnOutOfDateDatabase
	{
		[Test]
		public void ShouldDetermineIfUpgradeIsRequired ()
		{
			Assert.IsTrue(DbUpgrader.IsUpgradeRequired(Log));
		}
		
		[Test]
		public void ShouldLogInformation()
		{
			var result = DbUpgrader.PerformUpgrade(Log);
			var expectedScript = AllScripts.Last();
			
			ScriptExecutor.Received().Execute(ConnectionString, expectedScript, Log);
            VersionTracker.Received().StoreExecutedScript(ConnectionString, expectedScript, Log);
			
			Log.ReceivedWithAnyArgs().WriteInformation("");
			
			Assert.IsTrue(result.Successful);
			Assert.AreEqual(expectedScript, result.Scripts.First());
                    
		}
	}
}

