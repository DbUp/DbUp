
namespace DbUp.Oracle
{
    public class OracleCommandReader : DbUp.Support.SqlCommandReader, System.IDisposable
    {
        public OracleCommandReader(string sqlText) { }
        protected override bool IsCustomStatement { get; }
        protected override void ReadCustomStatement() { }
    }
    public class OracleCommandSplitter
    {
        public OracleCommandSplitter() { }
        public System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class OracleConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager, DbUp.Engine.Transactions.IConnectionManager
    {
        public OracleConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public static class OracleExtensions
    {
        public static DbUp.Builder.UpgradeEngineBuilder OracleDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
        public static DbUp.Builder.UpgradeEngineBuilder OracleDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema) { }
        public static DbUp.Builder.UpgradeEngineBuilder OracleDatabase(this DbUp.Builder.SupportedDatabases supported, DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
        public static DbUp.Builder.UpgradeEngineBuilder OracleDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
        public static DbUp.Builder.UpgradeEngineBuilder OracleDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager, string schema) { }
    }
    public class OracleObjectParser : DbUp.Support.SqlObjectParser, DbUp.Engine.ISqlObjectParser
    {
        public OracleObjectParser() { }
    }
    public class OraclePreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public OraclePreprocessor() { }
        public string Process(string contents) { }
    }
    public class OracleScriptExecutor : DbUp.Support.ScriptExecutor, DbUp.Engine.IScriptExecutor
    {
        public OracleScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journalFactory) { }
        public override void Execute(DbUp.Engine.SqlScript script) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class OracleTableJournal : DbUp.Support.TableJournal, DbUp.Engine.IJournal
    {
        public static System.Globalization.CultureInfo English;
        public OracleTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string schema, string table) { }
        protected string CreateSchemaTableSequenceSql() { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected string CreateSchemaTableTriggerSql() { }
        protected override string DoesTableExistSql() { }
        public override void EnsureTableExistsAndIsLatestVersion(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected System.Data.IDbCommand GetCreateTableSequence(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected System.Data.IDbCommand GetCreateTableTrigger(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
    }
}
