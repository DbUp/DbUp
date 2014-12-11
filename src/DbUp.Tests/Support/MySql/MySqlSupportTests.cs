using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DbUp.Tests.Support.MySql
{
    [TestFixture]
    public class MySqlSupportTests
    {
        private const string ConnectionString = "server=localhost;user id=root;password=;database=test";
        private Process mySqlProcess;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            StartMySql();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            DropAllTables();
            EndMySql();
            Thread.Sleep(1000);  // wait for mysql to exit
            CleanUpMySql();
        }

        [SetUp]
        public void Setup()
        {
            DropAllTables();
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

        private void StartMySql()
        {
            string path = Assembly.GetExecutingAssembly().Location;

            mySqlProcess = new Process
            {
                StartInfo =
                {
                    FileName = "..\\..\\..\\..\\lib\\DbUp.MySqlTestDatabase.1.0.0\\MySql\\bin\\mysqld.exe",
                    Arguments = "--console",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    WorkingDirectory = path.Substring(0, path.LastIndexOf("\\")) + "\\..\\..\\..\\..\\lib\\DbUp.MySqlTestDatabase.1.0.0\\MySql",
                    CreateNoWindow = false
                }
            };

            mySqlProcess.Start();
        }

        private void EndMySql()
        {
            mySqlProcess.Kill();
        }

        private void DropAllTables()
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

        private void CleanUpMySql()
        {
            var path = "..\\..\\..\\..\\lib\\DbUp.MySqlTestDatabase.1.0.0\\MySql\\data";

            var filesToDelete = Directory.EnumerateFiles(path, "ib_logfile*").ToList();
            filesToDelete.AddRange(Directory.EnumerateFiles(path, "ibdata*"));
            filesToDelete.Add(path + "\\mysql.err");
            filesToDelete.Add(path + "\\mysql-slow.log");
            filesToDelete.AddRange(Directory.EnumerateFiles(path, "*.pid"));

            foreach (var fileName in filesToDelete)
            {
                File.Delete(fileName);
            }
        }
    }
}
