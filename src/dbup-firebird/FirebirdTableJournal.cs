using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using System.Data;
using DbUp.Support;

namespace DbUp.Firebird
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// Firebird database using a table called SchemaVersions.
    /// </summary>
    public class FirebirdTableJournal : TableJournal
    {
        /// <summary>
        /// Creates a new Firebird table journal.
        /// </summary>
        /// <param name="connectionManager">The Firebird connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="tableName">The name of the journal table.</param>
        public FirebirdTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string tableName)
            : base(connectionManager, logger, new FirebirdObjectParser(), null, tableName)
        {
        }

        static string CreateGeneratorSql(string tableName)
        {
            return $@"
                CREATE SEQUENCE GEN_{tableName}ID
                ";
        }

        static string CreateTriggerSql(string tableName)
        {
            return $@"
                CREATE TRIGGER BI_{tableName}ID FOR {tableName} ACTIVE BEFORE INSERT POSITION 0 
                AS BEGIN
                    IF (new.SchemaVersionsId IS NULL OR (new.SchemaVersionsId = 0)) 
                    THEN new.SchemaVersionsId = GEN_ID(GEN_{tableName}ID,1);
                END;";
        }

        static string GeneratorName(string tableName) { return ""; }
        static string TriggerName(string tableName) { return ""; }

        void ExecuteCommand(Func<IDbCommand> dbCommandFactory, string sql)
        {
            using (var command = dbCommandFactory())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        protected override void OnTableCreated(Func<IDbCommand> dbCommandFactory)
        {
            var unqotedTableName = UnquoteSqlObjectName(FqSchemaTableName);

            ExecuteCommand(dbCommandFactory, CreateGeneratorSql(unqotedTableName));
            Log().WriteInformation($"The GEN_{unqotedTableName}ID generator has been created");

            ExecuteCommand(dbCommandFactory, CreateTriggerSql(unqotedTableName));
            Log().WriteInformation($"The BI_{unqotedTableName}ID trigger has been created");
        }

        protected override string DoesTableExistSql()
        {
            return $@"
                SELECT 1 FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0 AND RDB$RELATION_NAME = UPPER('{UnquotedSchemaTableName}')
                ";
        }

        protected override string GetInsertJournalEntrySql(string @scriptName, string @applied)
        {
            return $@" 
                INSERT INTO {UnquotedSchemaTableName} (ScriptName, Applied) VALUES ({scriptName}, {applied})
                ";
        }

        protected override string GetJournalEntriesSql()
        {
            return $@"
                SELECT ScriptName FROM {UnquotedSchemaTableName} ORDER BY ScriptName
                ";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return $@"
                CREATE TABLE {UnquotedSchemaTableName} (
                    SchemaVersionsId    INTEGER         NOT NULL,
                    ScriptName          VARCHAR(255)    NOT NULL,
                    Applied             TIMESTAMP       NOT NULL,
                    CONSTRAINT pk_{UnquotedSchemaTableName}_id PRIMARY KEY (SchemaVersionsId)
                )";
        }
    }
}
