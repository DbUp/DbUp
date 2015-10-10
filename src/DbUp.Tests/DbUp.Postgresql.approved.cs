[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("5ddc04cc-0bd3-421e-9ae4-9fd0e4f4ef04")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.0", FrameworkDisplayName=".NET Framework 4")]

namespace DbUp.Postgresql
{
    
    public class PostgresqlConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public PostgresqlConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class PostgresqlObjectParser : DbUp.Support.SqlObjectParser
    {
        public PostgresqlObjectParser() { }
        public override string QuoteIdentifier(string objectName, DbUp.Support.ObjectNameOptions objectNameOptions) { }
    }
    public class PostgresqlPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public PostgresqlPreprocessor() { }
        public string Process(string contents) { }
    }
    public class PostgresqlScriptExecutor : DbUp.Support.ScriptExecutor
    {
        public PostgresqlScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action excuteCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class PostgresqlTableJournal : DbUp.Support.TableJournal
    {
        public PostgresqlTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string schema, string table) { }
        protected override System.Data.IDbCommand GetCreateTableCommand(System.Func<System.Data.IDbCommand> dbCommandFactory, string schemaTableName) { }
        protected override System.Data.IDbCommand GetInsertScriptCommand(System.Func<System.Data.IDbCommand> dbCommandFactory, DbUp.Engine.SqlScript script) { }
        protected override System.Data.IDbCommand GetSelectExecutedScriptsCommand(System.Func<System.Data.IDbCommand> dbCommandFactory, string schemaTableName) { }
    }
}

public class static PostgresqlExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder PostgresqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder PostgresqlDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static void PostgresqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString) { }
    public static void PostgresqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger) { }
}