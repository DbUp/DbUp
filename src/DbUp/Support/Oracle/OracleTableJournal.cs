using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Support.Oracle
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for an 
    /// Oracle database using a table called SchemaVersions.
    /// </summary>
    public class OracleTableJournal : IJournal
    {
        private readonly string table;
        private readonly string tableName;
        private readonly Func<IConnectionManager> connectionManager;
        private readonly Func<IUpgradeLog> log;

        private static string QuoteIdentifier(string identifier)
        {
            return "\"" + identifier + "\"";
        }

        /// <summary>
        /// Creates a new Oracle table journal
        /// </summary>
        /// <param name="connectionMgr">The Oracle connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="journalTableName">The name of the journal table.</param>
        public OracleTableJournal(Func<IConnectionManager> connectionMgr, Func<IUpgradeLog> logger, string journalTableName)
        {
            table = journalTableName;
            tableName = QuoteIdentifier(journalTableName);
            connectionManager = connectionMgr;
            log = logger;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetExecutedScripts()
        {
            log().WriteInformation("Fetching list of already executed scripts.");
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", table));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = GetExecutedScriptsSql(tableName);
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            scripts.Add((string)reader[0]);
                    }
                }
            });

            return scripts.ToArray();
        }

        private static string GetExecutedScriptsSql(string journalTableName)
        {
            var scriptName = QuoteIdentifier("ScriptName");
            return string.Format("SELECT {0} FROM {1} ORDER BY {0}", scriptName, journalTableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        public void StoreExecutedScript(SqlScript script)
        {
            var scriptName = QuoteIdentifier("ScriptName");
            var appliedName = QuoteIdentifier("Applied");

            CreateJournalTableIfItDoesNotExits();

            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = string.Format("INSERT INTO {0} ({1}, {2}) VALUES (:SCRIPTNAME, :APPLIED)", tableName, scriptName, appliedName);

                    var scriptNameParam = command.CreateParameter();
                    scriptNameParam.ParameterName = "SCRIPTNAME";
                    scriptNameParam.Value = script.Name;
                    command.Parameters.Add(scriptNameParam);

                    var appliedParam = command.CreateParameter();
                    appliedParam.ParameterName = "APPLIED";
                    appliedParam.Value = DateTime.Now;
                    command.Parameters.Add(appliedParam);

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            });
        }

        private bool DoesTableExist()
        {
            return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        return VerifyTableExistsCommand(command);
                    }
                }
                catch (DbException)
                {
                    return false;
                }
            });
        }

        private bool VerifyTableExistsCommand(IDbCommand command)
        {
            command.CommandText = string.Format("select 1 from tab where tname = '{0}'", table);
            command.CommandType = CommandType.Text;
            var result = Convert.ToInt32(command.ExecuteScalar());
            return result == 1;
        }

        private void CreateJournalTableIfItDoesNotExits()
        {
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("Creating the {0} table", table));

                connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = CreateTableSql(table);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

                        command.CommandText = CreateTableSequence(table);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

                        command.CommandText = CreateTableTrigger(table);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    log().WriteInformation(string.Format("The {0} table has been created", tableName));
                });
            }
        }

        private static string CreateTableSql(string journalTableName)
        {
            var idColumn = QuoteIdentifier("SchemaVersionId");
            var primaryKey = QuoteIdentifier("SchemaVersionId_PK");
            var indexKey = QuoteIdentifier("SchemaVersionId_IX");
            var scriptColumn = QuoteIdentifier("ScriptName");
            var appliedColumn = QuoteIdentifier("Applied");
            var tableName = QuoteIdentifier(journalTableName);

            return string.Format(
                @"CREATE TABLE {5}
                    ( 
                        {0} NUMBER(10) NOT NULL, 
                        {1} VARCHAR2(255) NOT NULL, 
                        {2} TIMESTAMP NOT NULL, 
                            CONSTRAINT {3} PRIMARY KEY
                                ( {0} )
                            USING INDEX 
                                ( CREATE UNIQUE INDEX {4} ON {5} 
                                ({0} ASC) ) ENABLE 
                    )", idColumn, scriptColumn, appliedColumn, primaryKey, indexKey, tableName);
        }

        private static string CreateTableSequence(string journalTableName)
        {
            var sequenceName = QuoteIdentifier("SQ" + journalTableName);
            return string.Format(@"CREATE SEQUENCE  
                           {0}  
                           MINVALUE 1 
                           MAXVALUE 9999999999999999999999999999 
                           INCREMENT BY 1 
                           START WITH 21 
                           CACHE 20 
                           NOORDER
                           NOCYCLE", sequenceName);
        }

        private static string CreateTableTrigger(string journalTableName)
        {
            var idColumn = QuoteIdentifier("SchemaVersionId");
            var tableName = QuoteIdentifier(journalTableName);
            var sequenceName = QuoteIdentifier("SQ" + journalTableName);
            var triggerName = QuoteIdentifier("TR" + journalTableName);

            return string.Format("CREATE OR REPLACE TRIGGER {0} " +
                                 "BEFORE INSERT ON {1} " +
                                 "FOR EACH ROW " +
                                 "BEGIN " +
                                 "SELECT {2}.nextval INTO " +
                                 ":new.{3} " +
                                 "FROM dual; " +
                                 "END; ", triggerName, tableName, sequenceName, idColumn);
        }
    }
}