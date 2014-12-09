using System;
using NUnit.Framework;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DbUp.Tests.Support.MySql
{
    [TestFixture]
    public class MySqlSupportTests
    {
        private const string ConnectionString = "server=localhost;user id=username;password=password;database=test";

        [SetUp]
        public void Setup()
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(ConnectionString);
                conn.Open();

                var dropCommand = "DROP TABLE IF EXISTS Foo; DROP TABLE IF EXISTS Bar; DROP TABLE IF EXISTS schemaversions;";
                var cmd = new MySqlCommand(dropCommand, conn);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        [Test]
        public void CanUseMySql()
        {
            var upgrader = DeployChanges.To
                .MySqlDatabase(ConnectionString)
                .WithScript("Script0001", @"create table IF NOT EXISTS Foo (Id int(10))")
                .WithScript("Script0002", "create table IF NOT EXISTS Bar (Id int(10))")
                .Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
            Assert.IsTrue(TableExists("Foo"));
            Assert.IsTrue(TableExists("Bar"));
        }

        [Test]
        public void CanHandleDelimiter()
        {

            var upgrader = DeployChanges.To
                .MySqlDatabase(ConnectionString)
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

            Assert.IsTrue(result.Successful);
            Assert.IsTrue(SprocExists("testSproc"));
            

        }

        private bool TableExists(string tableName)
        {
            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(ConnectionString);
                conn.Open();

                var tableCommand = string.Format("SHOW TABLES LIKE '{0}'", tableName);
                var cmd = new MySqlCommand(tableCommand, conn);
                var result = cmd.ExecuteScalar();

                return (null != result);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        private bool SprocExists(string sprocName)
        {
            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(ConnectionString);
                conn.Open();

                var findCommand = string.Format("SHOW PROCEDURE STATUS WHERE name LIKE '{0}'", sprocName);
                var cmd = new MySqlCommand(findCommand, conn);
                var result = cmd.ExecuteScalar();

                return (null != result);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
