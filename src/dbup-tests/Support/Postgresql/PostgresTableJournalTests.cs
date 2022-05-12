using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Postgresql;
using DbUp.Tests.TestInfrastructure;
using Npgsql;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.Postgresql
{
    public class PostgresTableJournalTests
    {
        [Fact]
        public void uses_positional_parameters_when_creating_table()
        {
            // Arrange
            var dbConnection = Substitute.For<IDbConnection>();
            var connectionManager = new TestConnectionManager(dbConnection, true);
            var command = Substitute.For<IDbCommand>();
            var param1 = Substitute.For<IDbDataParameter>();
            var param2 = Substitute.For<IDbDataParameter>();
            dbConnection.CreateCommand().Returns(command);
            command.CreateParameter().Returns(param1, param2);
            command.ExecuteScalar().Returns(x => 0);
            var consoleUpgradeLog = new ConsoleUpgradeLog();
            var journal = new PostgresqlTableJournal(() => connectionManager, () => consoleUpgradeLog, "public", "SchemaVersions");

            // Act
            journal.StoreExecutedScript(new SqlScript("test", "select 1"), () => command);

            // Assert
            command.Received(2).CreateParameter();
            param1.ParameterName.ShouldBeNullOrEmpty();
            param2.ParameterName.ShouldBeNullOrEmpty();
            command.Received().ExecuteNonQuery();
        }
    }
}
