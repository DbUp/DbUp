[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("9f949414-f078-49bf-b50e-a3859c18fb6e")]

public static class SQLiteExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder SQLiteDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder SQLiteDatabase(this DbUp.Builder.SupportedDatabases supported, DbUp.SQLite.Helpers.SharedConnection sharedConnection) { }
}
namespace DbUp.SQLite
{
    public class SQLiteConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager, DbUp.Engine.Transactions.IConnectionManager
    {
        public SQLiteConnectionManager(string connectionString) { }
        public SQLiteConnectionManager(DbUp.SQLite.Helpers.SharedConnection sharedConnection) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SQLiteObjectParser : DbUp.Support.SqlObjectParser, DbUp.Engine.ISqlObjectParser
    {
        public SQLiteObjectParser() { }
    }
    public class SQLitePreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public SQLitePreprocessor() { }
        public string Process(string contents) { }
    }
    public class SQLiteScriptExecutor : DbUp.Support.ScriptExecutor, DbUp.Engine.IScriptExecutor
    {
        public SQLiteScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journalFactory) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class SQLiteTableJournal : DbUp.Support.TableJournal, DbUp.Engine.IJournal
    {
        public SQLiteTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string table) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override string DoesTableExistSql() { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
    }
}
namespace DbUp.SQLite.Helpers
{
    public class InMemorySQLiteDatabase : System.IDisposable
    {
        public InMemorySQLiteDatabase() { }
        public string ConnectionString { get; set; }
        public DbUp.Helpers.AdHocSqlRunner SqlRunner { get; }
        public void Dispose() { }
        public DbUp.Engine.Transactions.IConnectionManager GetConnectionManager() { }
    }
    public class SharedConnection : System.Data.IDbConnection, System.IDisposable
    {
        public SharedConnection(System.Data.IDbConnection dbConnection) { }
        public string ConnectionString { get; set; }
        public int ConnectionTimeout { get; }
        public string Database { get; }
        public System.Data.ConnectionState State { get; }
        public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) { }
        public System.Data.IDbTransaction BeginTransaction() { }
        public void ChangeDatabase(string databaseName) { }
        public void Close() { }
        public System.Data.IDbCommand CreateCommand() { }
        public void Dispose() { }
        public void DoClose() { }
        public void Open() { }
    }
    public class TemporarySQLiteDatabase : System.IDisposable
    {
        public TemporarySQLiteDatabase(string name) { }
        public DbUp.SQLite.Helpers.SharedConnection SharedConnection { get; }
        public DbUp.Helpers.AdHocSqlRunner SqlRunner { get; }
        public void Create() { }
        public void Dispose() { }
    }
}
