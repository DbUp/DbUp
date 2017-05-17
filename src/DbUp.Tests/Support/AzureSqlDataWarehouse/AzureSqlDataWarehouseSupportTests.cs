using System;
using ApprovalTests;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.AzureSqlDataWarehouse
{
    [TestFixture]
    public class AzureSqlDataWarehouseSupportTests
    {
        [Test]
        public void CanHandleDelimiter()
        {
            var recordingDbConnection = new RecordingDbConnection(true);
            var upgrader = DeployChanges.To
                .AzureSqlDataWarehouseDatabase(string.Empty)
                .OverrideConnectionFactory(recordingDbConnection)
                .WithScript("Script0003", @"
CREATE PROCEDURE test(id INT)
BEGIN 

    SELECT id      
    FROM   customer as c
    WHERE  c.id = ssn ; 
").Build();

            var result = upgrader.PerformUpgrade();

            result.Successful.ShouldBe(true);
            var commandLog = recordingDbConnection.GetCommandLog();
            try
            {
                Approvals.Verify(commandLog, Scrubbers.ScrubDates);
            }
            catch (Exception)
            {
                Console.WriteLine(commandLog);
                throw;
            }
        }
    }
}
