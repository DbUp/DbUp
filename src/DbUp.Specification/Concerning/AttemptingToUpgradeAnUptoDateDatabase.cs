using NUnit.Framework;
using System.Linq;

namespace DbUp.Specification.Concerning
{
    [TestFixture]
    public class AttemptingToUpgradeAnUptoDateDatabase : GivenAnUptoDateDatabase
    {
        [Test]
        public void ShouldNotRunAnyScripts()
        {
            var result = DbUpgrader.PerformUpgrade(Log);
            Assert.That(result.Scripts.Count() == 0);
        }
    }
}
