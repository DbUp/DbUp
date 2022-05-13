using System;
using System.Data;
using System.Linq;
using System.Runtime.Versioning;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Postgresql;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.Postgresql
{
    public class PostgresTableJournalTests
    {
        [Fact]
        public void uses_named_parameters_when_sql_rewriting_not_specified()
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
            param1.ParameterName.ShouldBe("scriptName");
            param2.ParameterName.ShouldBe("applied");
            command.CommandText.ShouldBe($"insert into \"public\".\"SchemaVersions\" (ScriptName, Applied) values (@scriptName, @applied)");
            command.Received().ExecuteNonQuery();
        }

#if NETSTANDARD1_3_OR_GREATER || NET46_OR_GREATER || NETCOREAPP
        [Fact]
#else
        [Fact(Skip="AppContext is not available for target platforms older than .NET Framework 4.6")]
#endif
        public void uses_positional_parameters_when_sql_rewriting_disabled()
        {

            var assemblyVersion = typeof(PostgresqlTableJournal).Assembly
                .CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(TargetFrameworkAttribute))
                .NamedArguments[0].TypedValue.Value.ToString();

            if (assemblyVersion == ".NET Framework 4.5")
            {
                // If the dbup-tests project uses .NET Framework 4.6 or 4.7, it defaults to choosing Framework 4.5 for the dbup-postgresql library.
                // This means AppContext is unavailable, so we can't disable SQL Rewriting.
                return;
            }


            // Arrange
            AppContext.SetSwitch("Npgsql.EnableSqlRewriting", false);

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
            command.CommandText.ShouldBe($"insert into \"public\".\"SchemaVersions\" (ScriptName, Applied) values ($1, $2)");
            command.Received().ExecuteNonQuery();
    }

#if NETSTANDARD1_3_OR_GREATER || NET46_OR_GREATER || NETCOREAPP
        [Fact]
#else
        [Fact(Skip="AppContext is not available for target platforms older than .NET Framework 4.6")]
#endif
        public void uses_named_parameters_when_sql_rewriting_enabled()
        {
            var assemblyVersion = typeof(PostgresqlTableJournal).Assembly
                .CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(TargetFrameworkAttribute))
                .NamedArguments[0].TypedValue.Value.ToString();

            if (assemblyVersion == ".NET Framework 4.5")
            {
                // If the dbup-tests project uses .NET Framework 4.6 or 4.7, it defaults to choosing Framework 4.5 for the dbup-postgresql library.
                // This means AppContext is unavailable, so we can't disable SQL Rewriting.
                return;
            }

            // Arrange
            AppContext.SetSwitch("Npgsql.EnableSqlRewriting", true);

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
            param1.ParameterName.ShouldBe("scriptName");
            param2.ParameterName.ShouldBe("applied");
            command.CommandText.ShouldBe($"insert into \"public\".\"SchemaVersions\" (ScriptName, Applied) values (@scriptName, @applied)");
            command.Received().ExecuteNonQuery();
        }
    }
}
