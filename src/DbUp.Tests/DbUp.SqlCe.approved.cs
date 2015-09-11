[assembly: System.CLSCompliantAttribute(true)]
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
    public class SqlCePreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public SqlCePreprocessor() { }
        public string Process(string contents) { }
    }
}

public class static SqlCeExtensions
{
    [System.ObsoleteAttribute("Pass connection string instead, then use .WithTransaction() and .WithTransactionP" +
        "erScript() to manage connection behaviour")]
    public static DbUp.Builder.UpgradeEngineBuilder SqlCeDatabase(this DbUp.Builder.SupportedDatabases supported, System.Func<System.Data.SqlServerCe.SqlCeConnection> connectionFactory) { }
    public static DbUp.Builder.UpgradeEngineBuilder SqlCeDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
}