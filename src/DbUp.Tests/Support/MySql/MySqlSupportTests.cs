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
            const string connectionString = "server=localhost;user id=username;password=password;database=test";

            var upgrader = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScript("Script0001", "create table Foo (Id int(10))")
                .Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }
    }
}
