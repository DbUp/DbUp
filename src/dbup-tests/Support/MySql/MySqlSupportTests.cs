using Assent;
using DbUp.Tests.TestInfrastructure;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.MySql
{
    public class MySqlSupportTests
    {
        [Fact]
        public void CanHandleDelimiter()
        {
            var logger = new CaptureLogsLogger();
            var recordingDbConnection = new RecordingDbConnection(logger, "schemaversions");
            recordingDbConnection.SetupRunScripts();
            var upgrader = DeployChanges.To
                .MySqlDatabase(string.Empty)
                .OverrideConnectionFactory(recordingDbConnection)
                .LogTo(logger)
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
            this.Assent(logger.Log, new Configuration().UsingSanitiser(Scrubbers.ScrubDates));

        }
    }
}