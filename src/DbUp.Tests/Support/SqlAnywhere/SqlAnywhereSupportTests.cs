using System;
using ApprovalTests;
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
                                        .SqlAnywhereDatabase("We don't care about connection string")
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
    }
}