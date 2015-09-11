[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("9f949414-f078-49bf-b50e-a3859c18fb6e")]

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
namespace DbUp.SQLite
{
    
    public class SQLiteConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public SQLiteConnectionManager(string connectionString) { }
        public SQLiteConnectionManager(DbUp.SQLite.Helpers.SharedConnection sharedConnection) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SQLitePreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public SQLitePreprocessor() { }
        public string Process(string contents) { }
    }
}

public class static SQLiteExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder SQLiteDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder SQLiteDatabase(this DbUp.Builder.SupportedDatabases supported, DbUp.SQLite.Helpers.SharedConnection sharedConnection) { }
}