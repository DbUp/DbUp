[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("8190b40b-ac5b-414f-8a00-9b6a2c12b010")]

public static class AzureSqlServerExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder AzureSqlDatabaseWithIntegratedSecurity(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema) { }
    public static DbUp.Builder.UpgradeEngineBuilder AzureSqlDatabaseWithIntegratedSecurity(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema, string resource) { }
    public static DbUp.Builder.UpgradeEngineBuilder AzureSqlDatabaseWithIntegratedSecurity(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema, string resource, string tenantId, string azureAdInstance = "https://login.microsoftonline.com/") { }
}
public static class SqlServerExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder JournalToSqlTable(this DbUp.Builder.UpgradeEngineBuilder builder, string schema, string table) { }
    public static DbUp.Builder.UpgradeEngineBuilder SqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder SqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema) { }
    public static DbUp.Builder.UpgradeEngineBuilder SqlDatabase(this DbUp.Builder.SupportedDatabases supported, DbUp.Engine.Transactions.IConnectionManager connectionManager, string schema = null) { }
    [System.ObsoleteAttribute("Use "AzureSqlDatabaseWithIntegratedSecurity(this SupportedDatabases, string, string)" if passing "true" to "useAzureSqlIntegratedSecurity".")]
    public static DbUp.Builder.UpgradeEngineBuilder SqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema, bool useAzureSqlIntegratedSecurity) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForDropDatabase supported, string connectionString) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.SqlServer.AzureDatabaseEdition azureDatabaseEdition) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, string collation) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForDropDatabase supported, string connectionString, int commandTimeout) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, DbUp.SqlServer.AzureDatabaseEdition azureDatabaseEdition) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, string collation) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.SqlServer.AzureDatabaseEdition azureDatabaseEdition, string collation) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForDropDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger, int timeout = -1) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, DbUp.SqlServer.AzureDatabaseEdition azureDatabaseEdition, string collation) { }
    public static void SqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger, int timeout = -1, DbUp.SqlServer.AzureDatabaseEdition azureDatabaseEdition = 0, string collation = null) { }
}
namespace DbUp.SqlServer
{
    public enum AzureDatabaseEdition : int
    {
        None = 0
        Basic = 1
        Standard = 2
        Premium = 3
    }
    public class AzureSqlConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager, DbUp.Engine.Transactions.IConnectionManager
    {
        public AzureSqlConnectionManager(string connectionString) { }
        public AzureSqlConnectionManager(string connectionString, string resource) { }
        public AzureSqlConnectionManager(string connectionString, string resource, string tenantId, string azureAdInstance = "https://login.microsoftonline.com/") { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SqlConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager, DbUp.Engine.Transactions.IConnectionManager
    {
        public SqlConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SqlScriptExecutor : DbUp.Support.ScriptExecutor, DbUp.Engine.IScriptExecutor
    {
        public SqlScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journalFactory) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class SqlServerObjectParser : DbUp.Support.SqlObjectParser, DbUp.Engine.ISqlObjectParser
    {
        public SqlServerObjectParser() { }
        public override string QuoteIdentifier(string objectName, DbUp.Support.ObjectNameOptions objectNameOptions) { }
    }
    public class SqlTableJournal : DbUp.Support.TableJournal, DbUp.Engine.IJournal
    {
        public SqlTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string schema, string table) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
    }
}
namespace DbUp.SqlServer.Helpers
{
    public class TemporarySqlDatabase : System.IDisposable
    {
        public TemporarySqlDatabase(string name) { }
        public TemporarySqlDatabase(Microsoft.Data.SqlClient.SqlConnectionStringBuilder connectionStringBuilder) { }
        public TemporarySqlDatabase(string name, string instanceName) { }
        public DbUp.Helpers.AdHocSqlRunner AdHoc { get; }
        public string ConnectionString { get; }
        public void Create() { }
        public void Dispose() { }
        public static DbUp.SqlServer.Helpers.TemporarySqlDatabase FromConnectionString(string connectionString) { }
    }
}
