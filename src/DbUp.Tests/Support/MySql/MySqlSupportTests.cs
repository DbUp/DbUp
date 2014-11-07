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
                .WithScript("Script0001", @"create table IF NOT EXISTS Foo (Id int(10))")
                .WithScript("Script0002", "create table IF NOT EXISTS Bar (Id int(10))")
                .Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }

        [Test]
        public void CanHandleDelimiter()
        {
            const string connectionString = "server=localhost;user id=username;password=password;database=test";

            var upgrader = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScript("Script0003", @"USE `test`;
DROP procedure IF EXISTS `CSF_LookupCustomerId`;

DELIMITER $$

USE `test`$$
CREATE PROCEDURE `CSF_LookupCustomerId`(
        IN   ssn                    VARCHAR(32)    
     )
BEGIN 

    SELECT id      
    FROM   customer as c
    WHERE  c.ssn = ssn ; 

END$$").Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }
    }
}
