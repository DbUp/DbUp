using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Support.SQLite;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace DbUp.Specification.SQLite
{
    [TestFixture]
    public class SQLiteTableJournalTests
    {
        [Test]
        public void dbversion_is_zero_when_journal_table_not_exist()
        {
            // Given
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            command.ExecuteScalar().Returns(x => { throw new SQLiteException("table not found"); });
            var journal = new SQLiteTableJournal(() => dbConnection, "SchemaVersions", new ConsoleUpgradeLog());

            // When
            var scripts = journal.GetExecutedScripts();

            // Expect
            command.DidNotReceive().ExecuteReader();
            Assert.AreEqual(0, scripts.Length);
        }

        [Test]
        public void creates_a_new_journal_table_when_not_exist()
        {
            // Given
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var param = Substitute.For<IDbDataParameter>();
            dbConnection.CreateCommand().Returns(command);
            command.CreateParameter().Returns(param);
            command.ExecuteScalar().Returns(x => { throw new SQLiteException("table not found"); });
            var journal = new SQLiteTableJournal(() => dbConnection, "SchemaVersions", new ConsoleUpgradeLog());

            // When
            journal.StoreExecutedScript(new SqlScript("test", "select 1"));

            // Expect
            command.Received().CreateParameter();
            Assert.AreEqual("scriptName", param.ParameterName);
            command.Received().ExecuteNonQuery();
        }
    }
}
