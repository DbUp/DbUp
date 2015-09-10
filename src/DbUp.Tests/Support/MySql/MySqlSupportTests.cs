using System;
using ApprovalTests;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.MySql
{
    [TestFixture]
    public class MySqlSupportTests
    {
        [Test]
        public void CanHandleDelimiter()
        {
            var recordingDbConnection = new RecordingDbConnection(true);
            var upgrader = DeployChanges.To
                .MySqlDatabase(string.Empty)
                .OverrideConnectionFactory(recordingDbConnection)
                .WithScript("Script0003", @"USE `test`;
DROP procedure IF EXISTS `testSproc`;

DELIMITER $$

USE `test`$$
CREATE PROCEDURE `testSproc`(
        IN   ssn                    VARCHAR(32)
     )
BEGIN 

    SELECT id      
    FROM   customer as c
    WHERE  c.ssn = ssn ; 

END$$").Build();

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
