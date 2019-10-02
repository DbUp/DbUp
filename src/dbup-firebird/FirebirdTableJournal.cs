using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
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
            return $@"CREATE SEQUENCE {GeneratorName(tableName)}";
        }

        static string CreateTriggerSql(string tableName)
        {
            return
$@"CREATE TRIGGER {TriggerName(tableName)} FOR {tableName} ACTIVE BEFORE INSERT POSITION 0 AS BEGIN
    if (new.schemaversionsid is null or (new.schemaversionsid = 0)) then new.schemaversionsid = gen_id({GeneratorName(tableName)},1);
END;";
        }

        static string GeneratorName(string tableName) => $"GEN_{tableName}ID";

        static string TriggerName(string tableName) => $"BI_{tableName}ID";

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
            var unquotedTableName = UnquoteSqlObjectName(FqSchemaTableName);
            ExecuteCommand(dbCommandFactory, CreateGeneratorSql(unquotedTableName));
            Log().WriteInformation($"The {GeneratorName(unquotedTableName)} generator has been created");
            ExecuteCommand(dbCommandFactory, CreateTriggerSql(unquotedTableName));
            Log().WriteInformation($"The {TriggerName(unquotedTableName)} trigger has been created");
        }

        protected override string DoesTableExistSql()
        {
            return $"select 1 from RDB$RELATIONS where RDB$SYSTEM_FLAG = 0 and RDB$RELATION_NAME = '{UnquotedSchemaTableName}'";
        }

        protected override string GetInsertJournalEntrySql(string @scriptName, string @applied)
        {
            return $"insert into {FqSchemaTableName} (ScriptName, Applied) values ({scriptName}, {applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select ScriptName from {FqSchemaTableName} order by ScriptName";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
$@"CREATE TABLE {FqSchemaTableName}
(
    schemaversionsid INTEGER NOT NULL,
    scriptname VARCHAR(255) NOT NULL,
    applied TIMESTAMP NOT NULL,
    CONSTRAINT pk_{UnquotedSchemaTableName}_id PRIMARY KEY (schemaversionsid)
)";
        }
    }
}
