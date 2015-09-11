[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("a0df89fb-5f3e-4690-af40-acf2816e89f1")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.0", FrameworkDisplayName=".NET Framework 4")]

namespace DbUp.Firebird
{
    
    public class FirebirdConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public FirebirdConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class FirebirdPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public FirebirdPreprocessor() { }
        public string Process(string contents) { }
    }
}

public class static FirebirdExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
}