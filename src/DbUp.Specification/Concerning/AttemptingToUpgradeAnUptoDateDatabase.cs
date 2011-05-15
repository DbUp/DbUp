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
        public void ShouldNotRequireUpgrades()
        {
            Assert.IsFalse(DbUpgrader.IsUpgradeRequired());
        }

        [Test]
        public void ShouldNotRunAnyScripts()
        {
            var result = DbUpgrader.PerformUpgrade();
            Assert.AreEqual(0, result.Scripts.Count());
        }

        [Test]
        public void ShouldReturnSuccess()
        {
            var result = DbUpgrader.PerformUpgrade(); 
            Assert.IsTrue(result.Successful);
        }

        [Test]
        public void ShouldLogNoAction()
        {
            DbUpgrader.PerformUpgrade();
           
            Log.Received().WriteInformation("Beginning database upgrade");
            Log.Received().WriteInformation("No new scripts need to be executed - completing.");
        }

        [Test]
        public void ShouldNotStoreAnyScripts()
        {
            DbUpgrader.PerformUpgrade();

            VersionTracker.DidNotReceiveWithAnyArgs().StoreExecutedScript(null);
        }
    }
}
