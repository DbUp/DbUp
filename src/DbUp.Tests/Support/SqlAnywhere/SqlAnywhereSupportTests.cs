using System;
using ApprovalTests;
using DbUp.Builder;
using DbUp.SqlAnywhere;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.SqlAnywhere
{
    [TestFixture]
    public class SqlAnywhereSupportTests
    {
        private const string MultilineSqlScript = @"CREATE TABLE test ( keycol int default autoincrement, datacol char(10));
                                                    CREATE TRIGGER insert_test AFTER INSERT ON test
                                                    REFERENCING NEW AS new_row
                                                    FOR EACH ROW
                                                    BEGIN
                                                    SET aicol=new_row.keycol;
                                                    END
                                                    GO

                                                    CREATE VARIABLE aicol int;
                                                    INSERT INTO test (datacol) VALUES ('mytest');
                                                    SELECT aicol FROM dummy;";

        [Test]
        public void CanUseSqlAnywhere()
        {
            var recordingDbConnection = new RecordingDbConnection(true);
            var upgrader = DeployChanges.To
                                        .SqlAnywhereDatabase("We don't care about connection string", "nor the schema name")
                                        .OverrideConnectionFactory(recordingDbConnection)
                                        .WithScript("ScriptName", MultilineSqlScript).Build();

            var result = upgrader.PerformUpgrade();

            result.Successful.ShouldBe(true);
            var commandLog = recordingDbConnection.GetCommandLog();
            try
            {
                Approvals.Verify(commandLog, Scrubbers.ScrubDates);
            }
            finally
            {
                Console.WriteLine(commandLog);
            }
        }

        [Test]
        public void Should_Configure_ScriptPreprocessors()
        {
            UpgradeConfiguration config = null;
            var builder = DeployChanges.To.SqlAnywhereDatabase("We don't care about connection string", "SchemaName").WithScriptsEmbeddedInAssembly(GetType().Assembly);

            config = ExtractConfigurationFromBuilder(builder);

            config.ScriptPreprocessors.ShouldContain(x => x.GetType() == typeof(SqlAnywhereSqlPreprocessor));
        }

        private static UpgradeConfiguration ExtractConfigurationFromBuilder(UpgradeEngineBuilder builder)
        {
            UpgradeConfiguration config = null;
            builder.Configure(c => config = c);
            builder.Build();
            return config;
        }
    }
}