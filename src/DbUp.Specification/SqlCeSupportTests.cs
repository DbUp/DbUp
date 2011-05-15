using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DbUp.Journal;
using DbUp.ScriptProviders;
using NUnit.Framework;

namespace DbUp.Specification
{
    [TestFixture]
    public class SqlCeSupportTests
    {
        [Test]
        public void CanUseSqlCe4()
        {
            const string connectionString = "Data Source=test.sdf; Persist Security Info=False";

            if (!File.Exists("test.sdf"))
            {
                var engine = new SqlCeEngine(connectionString);
                engine.CreateDatabase();
            }

            var scripts = new List<SqlScript>();
            scripts.Add(new SqlScript("Script0001", "create table Foo (Id int)"));
            
            var upgrader = new DatabaseUpgrader(() => new SqlCeConnection(connectionString), new StaticScriptProvider(scripts));
            upgrader.Journal = new TableJournal(() => new SqlCeConnection(connectionString), null);
            
            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }
    }
}
