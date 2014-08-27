using System;
using NUnit.Framework;

namespace DbUp.Tests.Oracle
{
    [TestFixture]
    public class OracleSupportTests
    {
        [Test]
        public void CanUseOracle()
        {

            string connectionString = @"Host=localhost;Direct=true;Service Name=pdborcl.si.corp.adacta-group.com;User ID=TESTNA;Password=adinsure;Unicode=true";
            
            var upgrader = DeployChanges.To
                .OracleDatabase(connectionString)
                .WithScript("Script0001", @"create table FOO (ID integer)")
                .Build();

            var result = upgrader.PerformUpgrade();

            Assert.IsTrue(result.Successful);
        }
    }
}
