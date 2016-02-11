﻿[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("2523f9cc-42c7-48da-b873-74851c335931")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.0", FrameworkDisplayName=".NET Framework 4")]

namespace DbUp.SqlCe
{
    
    public class SqlCeConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public SqlCeConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class SqlCeObjectParser : DbUp.Support.SqlObjectParser
    {
        public SqlCeObjectParser() { }
        public override string QuoteIdentifier(string objectName, DbUp.Support.ObjectNameOptions objectNameOptions) { }
    }
    public class SqlCePreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public SqlCePreprocessor() { }
        public string Process(string contents) { }
    }
    public class SqlCeScriptExecutor : DbUp.Support.ScriptExecutor
    {
        public SqlCeScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journal) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action excuteCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class SqlCeTableJournal : DbUp.Support.TableJournal
    {
        public SqlCeTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string schema, string table) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override string DoesTableExistSql() { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
    }
}

public class static SqlCeExtensions
{
    [System.ObsoleteAttribute("Pass connection string instead, then use .WithTransaction() and .WithTransactionP" +
        "erScript() to manage connection behaviour")]
    public static DbUp.Builder.UpgradeEngineBuilder SqlCeDatabase(this DbUp.Builder.SupportedDatabases supported, System.Func<System.Data.SqlServerCe.SqlCeConnection> connectionFactory) { }
    public static DbUp.Builder.UpgradeEngineBuilder SqlCeDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
}