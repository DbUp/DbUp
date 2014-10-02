using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.Tests.Support.Postgresql
{
    [TestFixture]
    public class PostgresqlSupportTests
    {
        [Test]
        public void CanUpgradeAPostgresqlDatabase()
        {
            //var upgrader = DeployChanges.To
            //    .PostgresqlDatabase("")
            //    .WithScript("Script0001", "create table $schema$.Foo (Id int)")
            //    .Build();

            //var result = upgrader.PerformUpgrade();

            //Assert.IsTrue(result.Successful);
        }
    }
}
