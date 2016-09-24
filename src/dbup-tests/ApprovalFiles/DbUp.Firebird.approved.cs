﻿[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("a0df89fb-5f3e-4690-af40-acf2816e89f1")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.0")]

namespace DbUp.Firebird
{
    
    public class FirebirdConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public FirebirdConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class FirebirdObjectParser : DbUp.Support.SqlObjectParser
    {
        public FirebirdObjectParser() { }
    }
    public class FirebirdPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public FirebirdPreprocessor() { }
        public string Process(string contents) { }
    }
    public class FirebirdScriptExecutor : DbUp.Support.ScriptExecutor
    {
        public FirebirdScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journal) { }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action excuteCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class FirebirdTableJournal : DbUp.Support.TableJournal
    {
        public FirebirdTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string tableName) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override string DoesTableExistSql() { }
        protected override string GetCreateBatchNumberColumnSql() { }
        protected override string GetDoesBatchNumberColumnExistSql() { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetInsertJournalEntryWithBatchNumberSql(string scriptName, string applied, string batchNumber) { }
        protected override string GetJournalEntriesSql() { }
        protected override string GetMaximumBatchNumberSql() { }
        protected override void OnTableCreated(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
    }
}

public class static FirebirdExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
}