#if !NETCORE
using System;
using System.Data.SqlServerCe;
using System.IO;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.SqlCe
{
    public class SqlCeSupportTests
    {
        [Fact]
        public void CanUseSqlCe4()
        {
            const string connectionString = "Data Source=test.sdf; Persist Security Info=False";

            if (!File.Exists("test.sdf"))
            {
                var engine = new SqlCeEngine(connectionString);
                engine.CreateDatabase();
            }

            //Verify supports scripts which specify schema (To Support SqlCe and Sql with Schemas)
            var upgrader = DeployChanges.To
                .SqlCeDatabase(connectionString)
                .WithScript("Script0001", "create table $schema$.Foo (Id int)")
                .Build();

            var result = upgrader.PerformUpgrade();

            result.Successful.ShouldBe(true);
        }
    }
}
#endif