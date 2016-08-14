[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("9f833e49-6e35-4e4d-b2a0-3d4fed527c89")]

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
        public System.Collections.Generic.List<DbUp.Engine.IScriptPreprocessor> ScriptPreprocessors { get; }
        public System.Collections.Generic.List<DbUp.Engine.IScriptProvider> ScriptProviders { get; }
        public System.Collections.Generic.Dictionary<string, string> Variables { get; }
        public bool VariablesEnabled { get; set; }
        public void AddVariables(System.Collections.Generic.IDictionary<string, string> newVariables) { }
        public void Validate() { }
    }
    public class UpgradeEngineBuilder
    {
        public UpgradeEngineBuilder() { }
        public DbUp.Engine.UpgradeEngine Build() { }
        public void Configure(System.Action<DbUp.Builder.UpgradeConfiguration> configuration) { }
    }
}
namespace DbUp
{
    
    public class static DbUpDefaults
    {
        public static System.Text.Encoding DefaultEncoding;
    }
    public class static DeployChanges
    {
        public static DbUp.Builder.SupportedDatabases To { get; }
    }
    public class static EnsureDatabase
    {
        public static DbUp.SupportedDatabasesForEnsureDatabase For { get; }
    }
    public class static Filters
    {
        public static System.Func<string, bool> ExcludeScriptNamesInFile(string fileName) { }
        public static System.Func<string, bool> ExcludeScripts(params string[] scriptNames) { }
        public static System.Func<string, bool> OnlyIncludeScriptNamesInFile(string fileName) { }
        public static System.Func<string, bool> OnlyIncludeScripts(params string[] scriptNames) { }
    }
    public class SupportedDatabasesForEnsureDatabase
    {
        public SupportedDatabasesForEnsureDatabase() { }
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
        public override string Contents { get; }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{Name}")]
    public class SqlScript
    {
        public SqlScript(string name, string contents) { }
        public virtual string Contents { get; }
        public string Name { get; }
        public static DbUp.Engine.SqlScript FromFile(string path) { }
        public static DbUp.Engine.SqlScript FromFile(string path, System.Text.Encoding encoding) { }
        public static DbUp.Engine.SqlScript FromStream(string scriptName, System.IO.Stream stream) { }
        public static DbUp.Engine.SqlScript FromStream(string scriptName, System.IO.Stream stream, System.Text.Encoding encoding) { }
    }
    public class UpgradeEngine
    {
        public UpgradeEngine(DbUp.Builder.UpgradeConfiguration configuration) { }
        public System.Collections.Generic.List<string> GetExecutedScripts() { }
        public System.Collections.Generic.List<DbUp.Engine.SqlScript> GetScriptsToExecute() { }
        public bool IsUpgradeRequired() { }
        public DbUp.Engine.DatabaseUpgradeResult MarkAsExecuted() { }
        public DbUp.Engine.DatabaseUpgradeResult MarkAsExecuted(string latestScript) { }
        public DbUp.Engine.DatabaseUpgradeResult PerformUpgrade() { }
        public bool TryConnect(out string errorMessage) { }
    }
}
namespace DbUp.Engine.Output
{
    
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
        public T ExecuteCommandsWithManagedConnection<T>(System.Func<System.Func<System.Data.IDbCommand>, T> actionWithResult) { }
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
        void ExecuteCommandsWithManagedConnection([JetBrains.Annotations.InstantHandleAttribute()] System.Action<System.Func<System.Data.IDbCommand>> action);
        T ExecuteCommandsWithManagedConnection<T>(System.Func<System.Func<System.Data.IDbCommand>, T> actionWithResult);
        System.IDisposable OperationStarting(DbUp.Engine.Output.IUpgradeLog upgradeLog, System.Collections.Generic.List<DbUp.Engine.SqlScript> executedScripts);
        System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents);
        bool TryConnect(DbUp.Engine.Output.IUpgradeLog upgradeLog, out string errorMessage);
    }
    public interface ITransactionStrategy : System.IDisposable
    {
        void Execute(System.Action<System.Func<System.Data.IDbCommand>> action);
        T Execute<T>(System.Func<System.Func<System.Data.IDbCommand>, T> actionWithResult);
        void Initialise(System.Data.IDbConnection dbConnection, DbUp.Engine.Output.IUpgradeLog upgradeLog, System.Collections.Generic.List<DbUp.Engine.SqlScript> executedScripts);
    }
    public class LegacySqlConnectionManager : DbUp.Engine.Transactions.IConnectionManager
    {
        public LegacySqlConnectionManager(System.Func<System.Data.IDbConnection> connectionFactory) { }
        public bool IsScriptOutputLogged { get; set; }
        public DbUp.Engine.Transactions.TransactionMode TransactionMode { get; set; }
        public void ExecuteCommandsWithManagedConnection(System.Action<System.Func<System.Data.IDbCommand>> action) { }
        public T ExecuteCommandsWithManagedConnection<T>(System.Func<System.Func<System.Data.IDbCommand>, T> actionWithResult) { }
        public System.IDisposable OperationStarting(DbUp.Engine.Output.IUpgradeLog upgradeLog, System.Collections.Generic.List<DbUp.Engine.SqlScript> executedScripts) { }
        public System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
        public bool TryConnect(DbUp.Engine.Output.IUpgradeLog upgradeLog, out string errorMessage) { }
    }
    public enum TransactionMode
    {
        NoTransaction = 0,
        SingleTransaction = 1,
        TransactionPerScript = 2,
    }
}
namespace DbUp.Helpers
{
    
    public class AdHocSqlRunner
    {
        public AdHocSqlRunner(System.Func<System.Data.IDbCommand> commandFactory, DbUp.Engine.ISqlObjectParser sqlObjectParser, string schema, params DbUp.Engine.IScriptPreprocessor[] additionalScriptPreprocessors) { }
        public AdHocSqlRunner(System.Func<System.Data.IDbCommand> commandFactory, DbUp.Engine.ISqlObjectParser sqlObjectParser, string schema, System.Func<bool> variablesEnabled, params DbUp.Engine.IScriptPreprocessor[] additionalScriptPreprocessors) { }
        public string Schema { get; set; }
        public int ExecuteNonQuery(string query, params System.Linq.Expressions.Expression<>[] parameters) { }
        public System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>> ExecuteReader(string query, params System.Linq.Expressions.Expression<>[] parameters) { }
        public object ExecuteScalar(string query, params System.Linq.Expressions.Expression<>[] parameters) { }
        public DbUp.Helpers.AdHocSqlRunner WithVariable(string variableName, string value) { }
    }
    public class NullJournal : DbUp.Engine.IJournal
    {
        public NullJournal() { }
        public string[] GetExecutedScripts() { }
        public void StoreExecutedScript(DbUp.Engine.SqlScript script, System.Func<System.Data.IDbCommand> dbCommandFactory) { }
    }
}
namespace DbUp.ScriptProviders
{
    
    public class EmbeddedScriptAndCodeProvider : DbUp.Engine.IScriptProvider
    {
        public EmbeddedScriptAndCodeProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    }
    public class EmbeddedScriptProvider : DbUp.ScriptProviders.EmbeddedScriptsProvider
    {
        public EmbeddedScriptProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
        public EmbeddedScriptProvider(System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    }
    public class EmbeddedScriptsProvider : DbUp.Engine.IScriptProvider
    {
        public EmbeddedScriptsProvider(System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
        public System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> GetScripts(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    }
    public class FileSystemScriptProvider : DbUp.Engine.IScriptProvider
    {
        public FileSystemScriptProvider(string directoryPath, System.Func<string, bool> filter = null) { }
        public FileSystemScriptProvider(string directoryPath, System.Text.Encoding encoding) { }
        public FileSystemScriptProvider(string directoryPath, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
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
    
    public enum ObjectNameOptions
    {
        None = 0,
        Trim = 1,
    }
    public abstract class ScriptExecutor : DbUp.Engine.IScriptExecutor
    {
        public ScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, DbUp.Engine.ISqlObjectParser sqlObjectParser, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journal) { }
        public System.Nullable<int> ExecutionTimeoutSeconds { get; set; }
        protected System.Func<DbUp.Engine.Output.IUpgradeLog> Log { get; }
        public string Schema { get; set; }
        public virtual void Execute(DbUp.Engine.SqlScript script) { }
        public virtual void Execute(DbUp.Engine.SqlScript script, System.Collections.Generic.IDictionary<string, string> variables) { }
        protected virtual void ExecuteAndLogOutput(System.Data.IDbCommand command) { }
        protected abstract void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action excuteCallback);
        protected virtual void ExecuteNonQuery(System.Data.IDbCommand command) { }
        protected abstract string GetVerifySchemaSql(string schema);
        protected virtual string PreprocessScriptContents(DbUp.Engine.SqlScript script, System.Collections.Generic.IDictionary<string, string> variables) { }
        protected string QuoteSqlObjectName(string objectName) { }
        public void VerifySchema() { }
        protected virtual void WriteReaderToLog(System.Data.IDataReader reader) { }
    }
    public class SqlCommandReader : System.IO.StringReader
    {
        protected const int FailedRead = -1;
        public SqlCommandReader(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = True) { }
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
        protected char PeekChar() { }
        public override int Read() { }
        public override int Read(char[] buffer, int index, int count) { }
        public void ReadAllCommands(System.Action<string> handleCommand) { }
        public override int ReadBlock(char[] buffer, int index, int count) { }
        protected virtual void ReadCustomStatement() { }
        public override string ReadLine() { }
        public override string ReadToEnd() { }
        protected bool TryPeek(int numberOfCharacters, out string result) { }
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
    public abstract class TableJournal : DbUp.Engine.IJournal
    {
        protected TableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, DbUp.Engine.ISqlObjectParser sqlObjectParser, string schema, string table) { }
        protected System.Func<DbUp.Engine.Transactions.IConnectionManager> ConnectionManager { get; }
        protected string FqSchemaTableName { get; }
        protected System.Func<DbUp.Engine.Output.IUpgradeLog> Log { get; }
        protected string SchemaTableSchema { get; }
        protected string UnquotedSchemaTableName { get; }
        protected abstract string CreateSchemaTableSql(string quotedPrimaryKeyName);
        protected bool DoesTableExist() { }
        protected virtual string DoesTableExistSql() { }
        protected void EnsureTableIsLatestVersion() { }
        protected System.Data.IDbCommand GetCreateTableCommand(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        public string[] GetExecutedScripts() { }
        protected abstract string GetInsertJournalEntrySql(string scriptName, string applied);
        protected System.Data.IDbCommand GetInsertScriptCommand(System.Func<System.Data.IDbCommand> dbCommandFactory, DbUp.Engine.SqlScript script) { }
        protected System.Data.IDbCommand GetJournalEntriesCommand(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected abstract string GetJournalEntriesSql();
        protected virtual void OnTableCreated(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        public void StoreExecutedScript(DbUp.Engine.SqlScript script, System.Func<System.Data.IDbCommand> dbCommandFactory) { }
        protected string UnquoteSqlObjectName(string quotedIdentifier) { }
    }
}

public class static StandardExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder JournalTo(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IJournal journal) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogScriptOutput(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogTo(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.Output.IUpgradeLog log) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogToConsole(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder LogToTrace(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithExecutionTimeout(this DbUp.Builder.UpgradeEngineBuilder builder, System.Nullable<System.TimeSpan> timeout) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithoutTransaction(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithPreprocessor(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IScriptPreprocessor preprocessor) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.SqlScript script) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScript(this DbUp.Builder.UpgradeEngineBuilder builder, string name, string contents) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, DbUp.Engine.IScriptProvider scriptProvider) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, System.Collections.Generic.IEnumerable<DbUp.Engine.SqlScript> scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScripts(this DbUp.Builder.UpgradeEngineBuilder builder, params DbUp.Engine.SqlScript[] scripts) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly[] assemblies, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this DbUp.Builder.UpgradeEngineBuilder builder, System.Reflection.Assembly assembly, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Func<string, bool> filter) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithScriptsFromFileSystem(this DbUp.Builder.UpgradeEngineBuilder builder, string path, System.Func<string, bool> filter, System.Text.Encoding encoding) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithTransaction(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithTransactionPerScript(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariable(this DbUp.Builder.UpgradeEngineBuilder builder, string variableName, string value) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariables(this DbUp.Builder.UpgradeEngineBuilder builder, System.Collections.Generic.IDictionary<string, string> variables) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariablesDisabled(this DbUp.Builder.UpgradeEngineBuilder builder) { }
    public static DbUp.Builder.UpgradeEngineBuilder WithVariablesEnabled(this DbUp.Builder.UpgradeEngineBuilder builder) { }
}