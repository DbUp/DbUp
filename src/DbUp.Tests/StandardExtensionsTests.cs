using DbUp.Builder;
using NUnit.Framework;

namespace DbUp.Tests.Helpers
{
    [TestFixture]
    public class StandardExtensionsTests
    {
        [Test]
        public void Should_set_logical_name()
        {
            var builder = new UpgradeEngineBuilder().SetMigrationLogicalName("foo");

            Assert.AreEqual("foo", builder.MigrationLogicalName);
        }
    }
}
