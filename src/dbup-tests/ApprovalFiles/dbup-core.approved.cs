[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("9f833e49-6e35-4e4d-b2a0-3d4fed527c89")]

public static class StandardExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder JournalTo(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IJournal journal) { }
    public static DbUp.Builder.UpgradeEngineBuilder JournalTo(this DbUp.Builder.UpgradeEngineBuilder builder, System.Func<System.Func<DbUp.Engine.Transactions.IConnectionManager>, System.Func<DbUp.Engine.Output.IUpgradeLog>, DbUp.Engine.IJournal> createJournal) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogScriptOutput(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogTo(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.Output.IUpgradeLog log) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogToAutodetectedLog(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogToConsole(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogToNowhere(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogToTrace(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder ResetConfiguredLoggers(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithExecutionTimeout(this DbUp.Builder.UpgradeEngineBuilder builder, System.Nullable<System.TimeSpan> timeout) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithFilter(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IScriptFilter filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithoutTransaction(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithPreprocessor(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IScriptPreprocessor preprocessor) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.SqlScript script) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, string name, string contents) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, string name, string contents, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, string name, DbUp.Engine.IScript script) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, string name, DbUp.Engine.IScript script, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptNameComparer(this DbUp.Builder.UpgradeEngineBuilder builder, System.Collections.Generic.IComparer<string> comparer) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IScriptProvider scriptProvider) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, params DbUp.Engine.SqlScript[] scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, params DbUp.Engine.IScript[] scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, System.Func<DbUp.Engine.IScript, string> namer, params DbUp.Engine.IScript[] scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, System.Func<DbUp.Engine.IScript, string> namer, DbUp.Engine.SqlScriptOptions sqlScriptOptions, params DbUp.Engine.IScript[] scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Func<string, bool> codeScriptFilter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Func<string, bool> codeScriptFilter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Func<string, bool> filter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Func<string, bool> filter, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, DbUp.ScriptProviders.FileSystemScriptOptions options) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithTransaction(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithTransactionAlwaysRollback(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithTransactionPerScript(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariable(this DbUp.Builder.UpgradeEngineBuilder builder, string variableName, string value) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariables(this DbUp.Builder.UpgradeEngineBuilder builder, System.Collections.Generic.IDictionary<string, string> variables) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariablesDisabled(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariablesEnabled(this DbUp.Builder.UpgradeEngineBuilder builder) { }
}
namespace DbUp
{
    public static class DbUpDefaults
    {
        public static System.Text.Encoding DefaultEncoding;
        public static int DefaultRunGroupOrder;
    }
    public static class DeployChanges
    {
        public static DbUp.Builder.SupportedDatabases To { get; }
    }
    public static class DropDatabase
    {
        public static DbUp.SupportedDatabasesForDropDatabase For { get; }
    }
    public static class EnsureDatabase
    {
        public static DbUp.SupportedDatabasesForEnsureDatabase For { get; }
    }
    public static class Filters
    {
        public static System.Func<string, bool> ExcludeScriptNamesInFile(string fileName) { }
        public static System.Func<string, bool> ExcludeScripts(params string[] scriptNames) { }
        public static System.Func<string, bool> OnlyIncludeScriptNamesInFile(string fileName) { }
        public static System.Func<string, bool> OnlyIncludeScripts(params string[] scriptNames) { }
    }
    public static class OctopusDeployExtensions
    {
        public static void WriteExecutedScriptsToOctopusTaskSummary(this DbUp.Engine.DatabaseUpgradeResult result) { }
    }
    public class SupportedDatabasesForDropDatabase
    {
        public SupportedDatabasesForDropDatabase() { }
    }
    public class SupportedDatabasesForEnsureDatabase
    {
        public SupportedDatabasesForEnsureDatabase() { }
    }
}
namespace DbUp.Builder
{
    public class SupportedDatabases
    {
        public SupportedDatabases() { }
    }
    public class UpgradeConfiguration
    {
        public UpgradeConfiguration() { }
        public DbUp.Engine.Transactions.IConnectionManager ConnectionManager { get; set; }
        public DbUp.Engine.IJournal Journal { get; set; }
        public DbUp.Engine.Output.IUpgradeLog Log { get; set; }
        public DbUp.Engine.IScriptExecutor ScriptExecutor { get; set; }
        public DbUp.Engine.IScriptFilter ScriptFilter { get; set; }
        public DbUp.Support.ScriptNameComparer ScriptNameComparer { get; set; }
        public System.Collections.Generic.List<DbUp.Engine.IScriptPreprocessor> ScriptPreprocessors { get; }
        public System.Collections.Generic.List<DbUp.Engine.IScriptProvider> ScriptProviders { get; }
        public System.Collections.Generic.Dictionary<string, string> Variables { get; }
        public bool VariablesEnabled { get; set; }
        public void AddLog(DbUp.Engine.Output.IUpgradeLog additionalLog) { }
        public void AddVariables(System.Collections.Generic.IDictionary<string, string> newVariables) { }
        public void Validate() { }
    }
    public class UpgradeEngineBuilder
    {
        public UpgradeEngineBuilder() { }
        public DbUp.Engine.UpgradeEngine Build() { }
        public DbUp.Builder.UpgradeConfiguration BuildConfiguration() { }
        public void Configure(System.Action<DbUp.Builder.UpgradeConfiguration> configuration) { }
    }
}
namespace DbUp.Engine
{
    public sealed class DatabaseUpgradeResult
    {
        public DatabaseUpgradeResult(System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> scripts, bool successful, System.Exception error) { }
        public System.Exception Error { get; }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> Scripts { get; }
        public bool Successful { get; }
    }
    public interface IJournal
    {
        void EnsureTableExistsAndIsLatestVersion(System.Func<System.Data.IDbCommand> dbCommandFactory);
        string[] GetExecutedScripts();
        void StoreExecutedScript(DbUp.Engine.SqlScript script, System.Func<System.Data.IDbCommand> dbCommandFactory);
    }
    public interface IScript
    {
        string ProvideScript(System.Func<System.Data.IDbCommand> dbCommandFactory);
    }
    public interface IScriptExecutor
    {
        System.Nullable<int> ExecutionTimeoutSeconds { get; set; }
        void Execute(DbUp.Engine.SqlScript script);
        void Execute(DbUp.Engine.SqlScript script, System.Collections.Generic.IDictionary<string, string> variables);
        void VerifySchema();
    }
    public interface IScriptFilter
    {
        System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> Filter(System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> sorted, System.Collections.Generic.HashSet<string> executedScriptNames, DbUp.Support.ScriptNameComparer comparer);
    }
    public interface IScriptPreprocessor
    {
        string Process(string contents);
    }
    public interface IScriptProvider
    {
        System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager);
    }
    public interface ISqlObjectParser
    {
        string QuoteIdentifier(string objectName);
        string QuoteIdentifier(string objectName, DbUp.Support.ObjectNameOptions objectNameOptions);
        string UnquoteIdentifier(string objectName);
    }
    public class LazySqlScript : DbUp.Engine.SqlScript
    {
        public LazySqlScript(string name, System.Func<string> contentProvider) { }
        public LazySqlScript(string name, DbUp.Engine.SqlScriptOptions sqlScriptOptions, System.Func<string> contentProvider) { }
        public override string Contents { get; }
    }
    public class ScriptExecutedEventArgs : System.EventArgs
    {
        public ScriptExecutedEventArgs(DbUp.Engine.SqlScript script, DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
        public DbUp.Engine.Transactions.IConnectionManager ConnectionManager { get; }
        public DbUp.Engine.SqlScript Script { get; }
    }
    public class SqlScript
    {
        public SqlScript(string name, string contents) { }
        public SqlScript(string name, string contents, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
        public virtual string Contents { get; }
        public string Name { get; }
        public DbUp.Engine.SqlScriptOptions SqlScriptOptions { get; }
        public static DbUp.Engine.SqlScript FromFile(string path) { }
        public static DbUp.Engine.SqlScript FromFile(string path, System.Text.Encoding encoding) { }
        public static DbUp.Engine.SqlScript FromFile(string basePath, string path, System.Text.Encoding encoding) { }
        public static DbUp.Engine.SqlScript FromFile(string basePath, string path, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
        public static DbUp.Engine.SqlScript FromStream(string scriptName, System.IO.Stream stream) { }
        public static DbUp.Engine.SqlScript FromStream(string scriptName, System.IO.Stream stream, System.Text.Encoding encoding) { }
        public static DbUp.Engine.SqlScript FromStream(string scriptName, System.IO.Stream stream, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    }
    public class SqlScriptOptions
    {
        public SqlScriptOptions() { }
        public int RunGroupOrder { get; set; }
        public DbUp.Support.ScriptType ScriptType { get; set; }
    }
    public class UpgradeEngine
    {
        public UpgradeEngine(DbUp.Builder.UpgradeConfiguration configuration) { }
        public event System.EventHandler ScriptExecuted;
        public System.Collections.Generic.List<DbUp.Engine.SqlScript> GetDiscoveredScripts() { }
        public System.Collections.Generic.List<string> GetExecutedButNotDiscoveredScripts() { }
        public System.Collections.Generic.List<string> GetExecutedScripts() { }
        public System.Collections.Generic.List<DbUp.Engine.SqlScript> GetScriptsToExecute() { }
        public bool IsUpgradeRequired() { }
        public DbUp.Engine.DatabaseUpgradeResult MarkAsExecuted() { }
        public DbUp.Engine.DatabaseUpgradeResult MarkAsExecuted(string latestScript) { }
        protected virtual void OnScriptExecuted(DbUp.Engine.ScriptExecutedEventArgs e) { }
        public DbUp.Engine.DatabaseUpgradeResult PerformUpgrade() { }
        public bool TryConnect(out string errorMessage) { }
    }
}
namespace DbUp.Engine.Filters
{
    public class DefaultScriptFilter : DbUp.Engine.IScriptFilter
    {
        public DefaultScriptFilter() { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> Filter(System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> sorted, System.Collections.Generic.HashSet<string> executedScriptNames, DbUp.Support.ScriptNameComparer comparer) { }
    }
}
namespace DbUp.Engine.Output
{
    public class AutodetectUpgradeLog : DbUp.Engine.Output.IUpgradeLog
    {
        public AutodetectUpgradeLog() { }
        public void WriteError(string format, params object[] args) { }
        public void WriteInformation(string format, params object[] args) { }
        public void WriteWarning(string format, params object[] args) { }
    }
    public class ConsoleUpgradeLog : DbUp.Engine.Output.IUpgradeLog
    {
        public ConsoleUpgradeLog() { }
        public void WriteError(string format, params object[] args) { }
        public void WriteInformation(string format, params object[] args) { }
        public void WriteWarning(string format, params object[] args) { }
    }
    public interface IUpgradeLog
    {
        void WriteError(string format, params object[] args);
        void WriteInformation(string format, params object[] args);
        void WriteWarning(string format, params object[] args);
    }
    public class MultipleUpgradeLog : DbUp.Engine.Output.IUpgradeLog
    {
        public MultipleUpgradeLog(params DbUp.Engine.Output.IUpgradeLog[] upgradeLogs) { }
        public void WriteError(string format, params object[] args) { }
        public void WriteInformation(string format, params object[] args) { }
        public void WriteWarning(string format, params object[] args) { }
    }
    public class NoOpUpgradeLog : DbUp.Engine.Output.IUpgradeLog
    {
        public NoOpUpgradeLog() { }
        public void WriteError(string format, params object[] args) { }
        public void WriteInformation(string format, params object[] args) { }
        public void WriteWarning(string format, params object[] args) { }
    }
    public class TraceUpgradeLog : DbUp.Engine.Output.IUpgradeLog
    {
        public TraceUpgradeLog() { }
        public void WriteError(string format, params object[] args) { }
        public void WriteInformation(string format, params object[] args) { }
        public void WriteWarning(string format, params object[] args) { }
    }
}
namespace DbUp.Engine.Preprocessors
{
    public class StripSchemaPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public StripSchemaPreprocessor() { }
        public string Process(string contents) { }
    }
    public class VariableSubstitutionPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public VariableSubstitutionPreprocessor(System.Collections.Generic.IDictionary<string, string> variables) { }
        public string Process(string contents) { }
    }
    public class VariableSubstitutionSqlParser : DbUp.Support.SqlParser, System.IDisposable
    {
        public VariableSubstitutionSqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) { }
        protected override bool IsCustomStatement { get; }
        protected virtual char VariableDelimiter { get; }
        protected override void ReadCustomStatement() { }
        public string ReplaceVariables(System.Collections.Generic.IDictionary<string, string> variables) { }
        protected virtual bool ValidVariableNameCharacter(char c) { }
    }
}
namespace DbUp.Engine.Transactions
{
    public abstract class DatabaseConnectionManager : DbUp.Engine.Transactions.IConnectionManager
    {
        protected DatabaseConnectionManager(System.Func<DbUp.Engine.Output.IUpgradeLog, System.Data.IDbConnection> connectionFactory) { }
        protected DatabaseConnectionManager(DbUp.Engine.Transactions.IConnectionFactory connectionFactory) { }
        public bool IsScriptOutputLogged { get; set; }
        public DbUp.Engine.Transactions.TransactionMode TransactionMode { get; set; }
        public void ExecuteCommandsWithManagedConnection(System.Action<System.Func<System.Data.IDbCommand>> action) { }
        public T ExecuteCommandsWithManagedConnection<T>(Func<System.Func<System.Data.IDbCommand>, T> actionWithResult) { }
        public System.IDisposable OperationStarting(DbUp.Engine.Output.IUpgradeLog upgradeLog, System.Collections.Generic.List<DbUp.Engine.SqlScript> executedScripts) { }
        public System.IDisposable OverrideFactoryForTest(DbUp.Engine.Transactions.IConnectionFactory connectionFactory) { }
        public abstract System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents);
        public bool TryConnect(DbUp.Engine.Output.IUpgradeLog upgradeLog, out string errorMessage) { }
    }
    public class DelegateConnectionFactory : DbUp.Engine.Transactions.IConnectionFactory
    {
        public DelegateConnectionFactory(System.Func<DbUp.Engine.Output.IUpgradeLog, System.Data.IDbConnection> createConnection) { }
        public DelegateConnectionFactory(System.Func<DbUp.Engine.Output.IUpgradeLog, DbUp.Engine.Transactions.DatabaseConnectionManager, System.Data.IDbConnection> createConnection) { }
        public System.Data.IDbConnection CreateConnection(DbUp.Engine.Output.IUpgradeLog upgradeLog, DbUp.Engine.Transactions.DatabaseConnectionManager databaseConnectionManager) { }
    }
    public interface IConnectionFactory
    {
        System.Data.IDbConnection CreateConnection(DbUp.Engine.Output.IUpgradeLog upgradeLog, DbUp.Engine.Transactions.DatabaseConnectionManager databaseConnectionManager);
    }
    public interface IConnectionManager
    {
        bool IsScriptOutputLogged { get; set; }
        DbUp.Engine.Transactions.TransactionMode TransactionMode { get; set; }
        void ExecuteCommandsWithManagedConnection(System.Action<System.Func<System.Data.IDbCommand>> action);
        T ExecuteCommandsWithManagedConnection<T>(Func<System.Func<System.Data.IDbCommand>, T> actionWithResult);
        System.IDisposable OperationStarting(DbUp.Engine.Output.IUpgradeLog upgradeLog, System.Collections.Generic.List<DbUp.Engine.SqlScript> executedScripts);
        System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents);
        bool TryConnect(DbUp.Engine.Output.IUpgradeLog upgradeLog, out string errorMessage);
    }
    public interface ITransactionStrategy : System.IDisposable
    {
        void Execute(System.Action<System.Func<System.Data.IDbCommand>> action);
        T Execute<T>(Func<System.Func<System.Data.IDbCommand>, T> actionWithResult);
        void Initialise(System.Data.IDbConnection dbConnection, DbUp.Engine.Output.IUpgradeLog upgradeLog, System.Collections.Generic.List<DbUp.Engine.SqlScript> executedScripts);
    }
    public enum TransactionMode : int
    {
        NoTransaction = 0
        SingleTransaction = 1
        TransactionPerScript = 2
        SingleTransactionAlwaysRollback = 3
    }
}
namespace DbUp.Helpers
{
    public class AdHocSqlRunner
    {
        public AdHocSqlRunner(System.Func<System.Data.IDbCommand> commandFactory, DbUp.Engine.ISqlObjectParser sqlObjectParser, string schema, params DbUp.Engine.IScriptPreprocessor[] additionalScriptPreprocessors) { }
        public AdHocSqlRunner(System.Func<System.Data.IDbCommand> commandFactory, DbUp.Engine.ISqlObjectParser sqlObjectParser, string schema, System.Func<bool> variablesEnabled, params DbUp.Engine.IScriptPreprocessor[] additionalScriptPreprocessors) { }
        public string Schema { get; set; }
        public int ExecuteNonQuery(string query, params System.Linq.Expressions.Expression<System.Func<string, object>>[] parameters) { }
        public System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>> ExecuteReader(string query, params System.Linq.Expressions.Expression<System.Func<string, object>>[] parameters) { }
        public object ExecuteScalar(string query, params System.Linq.Expressions.Expression<System.Func<string, object>>[] parameters) { }
        public DbUp.Helpers.AdHocSqlRunner WithVariable(string variableName, string value) { }
    }
    public class NullJournal : DbUp.Engine.IJournal
    {
        public NullJournal() { }
        public void EnsureTableExistsAndIsLatestVersion(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        public string[] GetExecutedScripts() { }
        public void StoreExecutedScript(DbUp.Engine.SqlScript script, System.Func<System.Data.IDbCommand> dbCommandFactory) { }
    }
    public static class UpgradeEngineHtmlReport
    {
        public static void GenerateUpgradeHtmlReport(this DbUp.Engine.UpgradeEngine upgradeEngine, string fullPath) { }
        public static void GenerateUpgradeHtmlReport(this DbUp.Engine.UpgradeEngine upgradeEngine, string fullPath, string serverName, string databaseName) { }
    }
}
namespace DbUp.ScriptProviders
{
    public class EmbeddedScriptAndCodeProvider : DbUp.Engine.IScriptProvider
    {
        public EmbeddedScriptAndCodeProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
        public EmbeddedScriptAndCodeProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Func<string, bool> codeScriptFilter) { }
        public EmbeddedScriptAndCodeProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
        public EmbeddedScriptAndCodeProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Func<string, bool> codeScriptFilter, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    }
    public class EmbeddedScriptProvider : DbUp.ScriptProviders.EmbeddedScriptsProvider, DbUp.Engine.IScriptProvider
    {
        public EmbeddedScriptProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
        public EmbeddedScriptProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
        public EmbeddedScriptProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
    }
    public class EmbeddedScriptsProvider : DbUp.Engine.IScriptProvider
    {
        public EmbeddedScriptsProvider(System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
        public EmbeddedScriptsProvider(System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, System.Text.Encoding encoding, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    }
    public class FileSystemScriptOptions
    {
        public FileSystemScriptOptions() { }
        public System.Text.Encoding Encoding { get; set; }
        public string[] Extensions { get; set; }
        public System.Func<string, bool> Filter { get; set; }
        public bool IncludeSubDirectories { get; set; }
    }
    public class FileSystemScriptProvider : DbUp.Engine.IScriptProvider
    {
        public FileSystemScriptProvider(string directoryPath) { }
        public FileSystemScriptProvider(string directoryPath, DbUp.ScriptProviders.FileSystemScriptOptions options) { }
        public FileSystemScriptProvider(string directoryPath, DbUp.ScriptProviders.FileSystemScriptOptions options, DbUp.Engine.SqlScriptOptions sqlScriptOptions) { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    }
    public sealed class StaticScriptProvider : DbUp.Engine.IScriptProvider
    {
        public StaticScriptProvider(System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> scripts) { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    }
}
namespace DbUp.Support
{
    public enum ObjectNameOptions : int
    {
        None = 0
        Trim = 1
    }
    public abstract class ScriptExecutor : DbUp.Engine.IScriptExecutor
    {
        public ScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, DbUp.Engine.ISqlObjectParser sqlObjectParser, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journalFactory) { }
        public System.Nullable<int> ExecutionTimeoutSeconds { get; set; }
        protected System.Func<DbUp.Engine.Output.IUpgradeLog> Log { get; }
        public string Schema { get; set; }
        public virtual void Execute(DbUp.Engine.SqlScript script) { }
        public virtual void Execute(DbUp.Engine.SqlScript script, System.Collections.Generic.IDictionary<string, string> variables) { }
        protected virtual void ExecuteAndLogOutput(System.Data.IDbCommand command) { }
        protected abstract void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCallback);
        protected virtual void ExecuteNonQuery(System.Data.IDbCommand command) { }
        protected abstract string GetVerifySchemaSql(string schema);
        protected virtual string PreprocessScriptContents(DbUp.Engine.SqlScript script, System.Collections.Generic.IDictionary<string, string> variables) { }
        protected string QuoteSqlObjectName(string objectName) { }
        public void VerifySchema() { }
        protected virtual void WriteReaderToLog(System.Data.IDataReader reader) { }
    }
    public class ScriptNameComparer : System.Collections.Generic.IComparer<string>, System.Collections.Generic.IEqualityComparer<string>
    {
        public ScriptNameComparer(System.Collections.Generic.IComparer<string> comparer) { }
        public int Compare(string x, string y) { }
        public bool Equals(string x, string y) { }
        public int GetHashCode(string obj) { }
    }
    public enum ScriptType : int
    {
        RunOnce = 0
        RunAlways = 1
    }
    public class SqlCommandReader : DbUp.Support.SqlParser, System.IDisposable
    {
        public SqlCommandReader(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) { }
        public void ReadAllCommands(System.Action<string> handleCommand) { }
    }
    public class SqlCommandSplitter
    {
        public SqlCommandSplitter() { }
        public virtual System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public abstract class SqlObjectParser : DbUp.Engine.ISqlObjectParser
    {
        protected SqlObjectParser(string quotePrefix, string quoteSuffix) { }
        public string QuoteIdentifier(string objectName) { }
        public virtual string QuoteIdentifier(string objectName, DbUp.Support.ObjectNameOptions objectNameOptions) { }
        public virtual string UnquoteIdentifier(string objectName) { }
    }
    public abstract class SqlParser : System.IO.StringReader, System.IDisposable
    {
        protected const int FailedRead = -1;
        public SqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) { }
        protected event System.Action CommandEnded;
        protected event System.Action<DbUp.Support.SqlParser.CharacterType, char> ReadCharacter;
        protected char CurrentChar { get; }
        protected int CurrentIndex { get; }
        protected string Delimiter { get; set; }
        protected bool DelimiterRequiresWhitespace { get; set; }
        protected bool HasReachedEnd { get; }
        protected virtual bool IsCustomStatement { get; }
        protected bool IsEndOfLine { get; }
        protected bool IsQuote { get; }
        protected bool IsWhiteSpace { get; }
        protected char LastChar { get; }
        protected bool IsCharEqualTo(char comparisonChar, char compareTo) { }
        protected bool IsCurrentCharEqualTo(char comparisonChar) { }
        protected bool IsLastCharEqualTo(char comparisonChar) { }
        protected void OnReadCharacter(DbUp.Support.SqlParser.CharacterType type, char c) { }
        protected void Parse() { }
        protected char PeekChar() { }
        public override int Read() { }
        public override int Read(char[] buffer, int index, int count) { }
        public override int ReadBlock(char[] buffer, int index, int count) { }
        protected virtual void ReadCustomStatement() { }
        public override string ReadLine() { }
        public override string ReadToEnd() { }
        protected bool TryPeek(int numberOfCharacters, out string result) { }
        public enum CharacterType : int
        {
            Command = 0
            SlashStarComment = 1
            DashComment = 2
            BracketedText = 3
            QuotedString = 4
            Delimiter = 5
            CustomStatement = 6
        }
    }
    public abstract class TableJournal : DbUp.Engine.IJournal
    {
        protected TableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, DbUp.Engine.ISqlObjectParser sqlObjectParser, string schema, string table) { }
        protected System.Func<DbUp.Engine.Transactions.IConnectionManager> ConnectionManager { get; }
        protected string FqSchemaTableName { get; }
        protected System.Func<DbUp.Engine.Output.IUpgradeLog> Log { get; }
        protected string SchemaTableSchema { get; }
        protected string UnquotedSchemaTableName { get; }
        protected abstract string CreateSchemaTableSql(string quotedPrimaryKeyName);
        protected bool DoesTableExist(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected virtual string DoesTableExistSql() { }
        public virtual void EnsureTableExistsAndIsLatestVersion(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected System.Data.IDbCommand GetCreateTableCommand(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        public string[] GetExecutedScripts() { }
        protected abstract string GetInsertJournalEntrySql(string scriptName, string applied);
        protected System.Data.IDbCommand GetInsertScriptCommand(System.Func<System.Data.IDbCommand> dbCommandFactory, DbUp.Engine.SqlScript script) { }
        protected System.Data.IDbCommand GetJournalEntriesCommand(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected abstract string GetJournalEntriesSql();
        protected virtual void OnTableCreated(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        public virtual void StoreExecutedScript(DbUp.Engine.SqlScript script, System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected string UnquoteSqlObjectName(string quotedIdentifier) { }
    }
}
