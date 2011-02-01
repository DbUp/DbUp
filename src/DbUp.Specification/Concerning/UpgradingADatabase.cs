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
			var expectedScript = AllScripts.Last ();
			
			ScriptExecutor.Received ().Execute (ConnectionString, expectedScript, Log);
			VersionTracker.Received ().StoreExecutedScript (ConnectionString, expectedScript, Log);
			
			Log.ReceivedWithAnyArgs ().WriteInformation ("");
			
			Assert.IsTrue (result.Successful);
			Assert.AreEqual (expectedScript, result.Scripts.First ());
		}
		
		[Test]
		public void ShouldLogInformation ()
		{
			DbUpgrader.PerformUpgrade (Log);
			
			Log.Received ().WriteInformation ("Upgrade successful");
			Log.Received ().WriteInformation ("Beginning database upgrade. Connection string is: '{0}'", ConnectionString);
			
		}
	}
}

