using System;
using DbUp.MySql;
using NUnit.Framework;

namespace DbUp.Tests.Support.MySql
{
    [TestFixture]
    public class MySqlSupportTests
    {
        [Test]
        public void CanUseMySql()
        {
            const string connectionString = "server=localhost;user id=CCFI_Bus;password=jGUpYqS44SHN2hSw;database=test_ecash";

            var upgrader = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScript("Script0001", "create table Foo (Id int(10))")
                .Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }
    }
}
