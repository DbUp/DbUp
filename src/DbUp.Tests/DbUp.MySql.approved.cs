[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("b6988607-c547-4cbd-8012-f8162a25092f")]

namespace DbUp.MySql
{
    
    public class MySqlCommandReader : DbUp.Support.SqlServer.SqlCommandReader
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
    public class MySqlConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public MySqlConnectionManager(string connectionString) { }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class MySqlPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public MySqlPreprocessor() { }
        public string Process(string contents) { }
    }
}

public class static MySqlExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString, string schema) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static DbUp.Builder.UpgradeEngineBuilder MySqlDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager, string schema) { }
}