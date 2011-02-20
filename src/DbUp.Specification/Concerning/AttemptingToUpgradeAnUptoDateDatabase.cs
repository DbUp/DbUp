using NUnit.Framework;
using System.Linq;
using NSubstitute;
using NSubstitute.Core.Arguments;

namespace DbUp.Specification.Concerning
{
    [TestFixture]
    public class AttemptingToUpgradeAnUptoDateDatabase : GivenAnUptoDateDatabase
    {
        [Test]
        public void ShouldNotRunAnyScripts()
        {
            var result = DbUpgrader.PerformUpgrade(Log);
            Assert.AreEqual(0, result.Scripts.Count());
        }

        [Test]
        public void ShouldReturnSuccess()
        {
            var result = DbUpgrader.PerformUpgrade(Log); 
            Assert.IsTrue(result.Successful);
        }

        [Test]
        public void ShouldLogNoAction()
        {
            DbUpgrader.PerformUpgrade(Log);
           
            Log.Received().WriteInformation("Beginning database upgrade. Connection string is: '{0}'", ConnectionString);
            Log.Received().WriteInformation("No new scripts need to be executed - completing.");
        }

        [Test]
        public void ShouldNotStoreAnyScripts()
        {
            DbUpgrader.PerformUpgrade(Log);

            VersionTracker.DidNotReceiveWithAnyArgs().StoreExecutedScript(null, null);
        }
    }
}
