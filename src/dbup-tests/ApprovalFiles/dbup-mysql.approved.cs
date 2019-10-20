[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("b6988607-c547-4cbd-8012-f8162a25092f")]

public static class MySqlExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(this DbUp.Builder.SupportedDatabases supported, DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager, string schema) { }
    public static void MySqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString) { }
    public static void MySqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout) { }
    public static void MySqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, string collation) { }
    public static void MySqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, string collation) { }
    public static void MySqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger, int timeout = -1, string collation = null) { }
}
namespace DbUp.MySql
{
    public class MySqlCommandReader : DbUp.Support.SqlCommandReader, System.IDisposable
    {
        public MySqlCommandReader(string sqlText) { }
        protected override bool IsCustomStatement { get; }
        protected override void ReadCustomStatement() { }
    }
    public class MySqlCommandSplitter
    {
        public MySqlCommandSplitter() { }
        public System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class MySqlConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager, DbUp.Engine.Transactions.IConnectionManager
    {
        public MySqlConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class MySqlObjectParser : DbUp.Support.SqlObjectParser, DbUp.Engine.ISqlObjectParser
    {
        public MySqlObjectParser() { }
    }
    public class MySqlPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public MySqlPreprocessor() { }
        public string Process(string contents) { }
    }
    public class MySqlScriptExecutor : DbUp.Support.ScriptExecutor, DbUp.Engine.IScriptExecutor
    {
        public MySqlScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journalFactory) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class MySqlTableJournal : DbUp.Support.TableJournal, DbUp.Engine.IJournal
    {
        public MySqlTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string schema, string table) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
    }
}
