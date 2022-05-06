using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Google.Cloud.Spanner.Data;

namespace DbUp.Spanner
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// Spanner database using a table called SchemaVersions.
    /// </summary>
    public class SpannerTableJournal : TableJournal
    {
        /// <summary>
        /// Creates a new Spanner table journal.
        /// </summary>
        /// <param name="connectionManager">The Spanner connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="tableName">The name of the journal table.</param>
        public SpannerTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string tableName)
            : base(connectionManager, logger, new SpannerObjectParser(), schema, tableName)
        {
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string applied)
        {
            throw new NotImplementedException("Method not required for Google Cloud Spanner implementation.");
        }

        // An override is required as new connection is needed; can't query table while r/w transaction is active
        protected override bool DoesTableExist(Func<IDbCommand> dbCommandFactory)
        {
            Log().WriteInformation("Google Cloud Spanner: Checking whether journal table exists..");

            using (var command = dbCommandFactory())
            {
                using (IDbConnection dbConnection = command.Connection)
                {
                    var commandText = DoesTableExistSql();
                    using (var dbCommand = dbConnection.CreateCommand())
                    {
                        dbCommand.CommandText = commandText;
                        dbCommand.CommandType = CommandType.Text;
                        var executeScalar = dbCommand.ExecuteScalar();
                        if (executeScalar == null)
                            return false;
                        if (executeScalar is long)
                            return (long)executeScalar == 1;
                        if (executeScalar is decimal)
                            return (decimal)executeScalar == 1;
                        return (int)executeScalar == 1;
                    }
                }
            }
        }

        public override void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
        {
            EnsureTableExistsAndIsLatestVersion(dbCommandFactory);

            using (var command = GetInsertScriptCommand(dbCommandFactory, script))
            {
                command.ExecuteNonQuery();
            }
        }

        private new IDbCommand GetInsertScriptCommand(Func<IDbCommand> dbCommandFactory, SqlScript script)
        {
            var command = dbCommandFactory();

            var scriptId = command.CreateParameter();
            scriptId.ParameterName = "schemaversionsid";
            scriptId.Value = Guid.NewGuid().ToByteArray();
            command.Parameters.Add(scriptId);

            var scriptNameParam = command.CreateParameter();
            scriptNameParam.ParameterName = "scriptName";
            scriptNameParam.Value = script.Name;
            command.Parameters.Add(scriptNameParam);

            var appliedParam = command.CreateParameter();
            appliedParam.ParameterName = "applied";
            appliedParam.Value = DateTime.Now;
            command.Parameters.Add(appliedParam);

            command.CommandText = $"insert into {FqSchemaTableName} (schemaversionsid, scriptname, applied) values (@schemaversionsid,@scriptName,@applied)";
            command.CommandType = CommandType.Text;
            return command;
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select scriptname from {FqSchemaTableName} order by scriptname";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
                $@"CREATE TABLE {FqSchemaTableName}
(
    schemaversionsid BYTES(20) NOT NULL,
    scriptname STRING(255) NOT NULL,
    applied TIMESTAMP NOT NULL
) PRIMARY KEY (schemaversionsid)";
        }
    }
}
