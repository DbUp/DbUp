using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.Oracle
{
    public class OracleTableJournal : TableJournal
    {
        bool journalExists;
        /// <summary>
        /// Creates a new Oracle table journal.
        /// </summary>
        /// <param name="connectionManager">The Oracle connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="table">The name of the journal table.</param>
        public OracleTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new OracleObjectParser(), schema, table)
        {
        }

        public static CultureInfo English = new CultureInfo("en-US", false);

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            var fqSchemaTableName = this.UnquotedSchemaTableName;
            return
                $@" CREATE TABLE {fqSchemaTableName} 
                (
                    schemaversionid NUMBER(10),
                    scriptname VARCHAR2(255) NOT NULL,
                    applied TIMESTAMP NOT NULL,
                    CONSTRAINT PK_{ fqSchemaTableName } PRIMARY KEY (schemaversionid) 
                )";
        }

        protected string CreateSchemaTableSequenceSql()
        {
            var fqSchemaTableName = this.UnquotedSchemaTableName;
            return $@" CREATE SEQUENCE {fqSchemaTableName}_sequence";
        }

        protected string CreateSchemaTableTriggerSql()
        {
            var fqSchemaTableName = this.UnquotedSchemaTableName;
            return $@" CREATE OR REPLACE TRIGGER {fqSchemaTableName}_on_insert
                    BEFORE INSERT ON {fqSchemaTableName}
                    FOR EACH ROW
                    BEGIN
                        SELECT {fqSchemaTableName}_sequence.nextval
                        INTO :new.schemaversionid
                        FROM dual;
                    END;
                ";
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string applied)
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"insert into {unquotedSchemaTableName} (ScriptName, Applied) values (:" + scriptName.Replace("@","") + ",:" + applied.Replace("@","") + ")";
        }

        protected override string GetJournalEntriesSql()
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"select scriptname from {unquotedSchemaTableName} order by scriptname";
        }

        protected override string DoesTableExistSql()
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"select 1 from user_tables where table_name = '{unquotedSchemaTableName}'";
        }

        protected IDbCommand GetCreateTableSequence(Func<IDbCommand> dbCommandFactory)
        {
            var command = dbCommandFactory();
            command.CommandText = CreateSchemaTableSequenceSql();
            command.CommandType = CommandType.Text;
            return command;
        }

        protected IDbCommand GetCreateTableTrigger(Func<IDbCommand> dbCommandFactory)
        {
            var command = dbCommandFactory();
            command.CommandText = CreateSchemaTableTriggerSql();
            command.CommandType = CommandType.Text;
            return command;
        }

        public override void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        {
            if (!journalExists && !DoesTableExist(dbCommandFactory))
            {
                Log().WriteInformation(string.Format("Creating the {0} table", FqSchemaTableName));

                // We will never change the schema of the initial table create.
                using (var command = GetCreateTableSequence(dbCommandFactory))
                {
                    command.ExecuteNonQuery();
                }

                // We will never change the schema of the initial table create.
                using (var command = GetCreateTableCommand(dbCommandFactory))
                {
                    command.ExecuteNonQuery();
                }

                // We will never change the schema of the initial table create.
                using (var command = GetCreateTableTrigger(dbCommandFactory))
                {
                    command.ExecuteNonQuery();
                }

                Log().WriteInformation(string.Format("The {0} table has been created", FqSchemaTableName));

                OnTableCreated(dbCommandFactory);
            }

            journalExists = true;
        }
    }
}
