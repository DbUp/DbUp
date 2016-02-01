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
    public class PostgresqlPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public PostgresqlPreprocessor() { }
        public string Process(string contents) { }
    }
}

public class static PostgresqlExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder PostgresqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder PostgresqlDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static void PostgresqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString) { }
    public static void PostgresqlDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger) { }
}