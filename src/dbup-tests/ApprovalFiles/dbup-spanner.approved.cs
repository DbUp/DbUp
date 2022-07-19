
public static class SpannerExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder SpannerDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager, string schema) { }
    public static DbUp.Builder.UpgradeEngineBuilder SpannerDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema = null) { }
}
namespace DbUp.Spanner
{
    public class SpannerCommandReader : DbUp.Support.SqlCommandReader, System.IDisposable
    {
        public SpannerCommandReader(string sqlText) { }
    }
    public class SpannerCommandSplitter
    {
        public SpannerCommandSplitter() { }
        public System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SpannerConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager, DbUp.Engine.Transactions.IConnectionManager
    {
        public SpannerConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SpannerObjectParser : DbUp.Support.SqlObjectParser, DbUp.Engine.ISqlObjectParser
    {
        public SpannerObjectParser() { }
    }
    public class SpannerPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public SpannerPreprocessor() { }
        public string Process(string contents) { }
    }
    public class SpannerScriptExecutor : DbUp.Support.ScriptExecutor, DbUp.Engine.IScriptExecutor
    {
        public SpannerScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journalFactory) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class SpannerTableJournal : DbUp.Support.TableJournal, DbUp.Engine.IJournal
    {
        public SpannerTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string schema, string tableName) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override bool DoesTableExist(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
        public override void StoreExecutedScript(DbUp.Engine.SqlScript script, System.Func<System.Data.IDbCommand> dbCommandFactory) { }
    }
}
