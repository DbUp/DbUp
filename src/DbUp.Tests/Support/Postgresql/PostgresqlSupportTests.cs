using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DbUp.Tests.Support.Postgresql
{
    [TestFixture]
    public class PostgresqlSupportTests
    {
        [Test]
        public void CanUpgradeAPostgresqlDatabase()
        {
            var upgrader = DeployChanges.To
                .PostgresqlDatabase("Server=127.0.0.1;Database=testo;Port=5432;User Id=liam;Password=password;")
                .WithScript("Script0001", "create table Foo (Id int)")
                .Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }

        [Test]
        public void CanSetupTheDellDb()
        {
            var u = DeployChanges.To
                .PostgresqlDatabase("Server=127.0.0.1;Database=DS2;Port=5432;User Id=liam;Password=password;")
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();
            var result = u.PerformUpgrade();
        }
    }
}
