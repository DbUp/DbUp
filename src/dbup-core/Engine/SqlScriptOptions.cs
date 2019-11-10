using DbUp.Support;

namespace DbUp.Engine
{
    /// <summary>
    /// This class will allow you to set various options for the SQL Script Model and any child models.
    /// </summary>
    public class SqlScriptOptions
    {
        /// <summary>
        /// Get or Set the Script Type
        /// </summary>
        public ScriptType ScriptType { get; set; }

        /// <summary>
        /// Get or Set the Run Group Order, use this when you want to group together a batch of scripts to run in a specific order
        /// </summary>
        public int RunGroupOrder { get; set; }

        /// <summary>
        /// Default Constructor for the options
        /// </summary>
        public SqlScriptOptions()
        {
            RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder;
            ScriptType = ScriptType.RunOnce;
        }
    }
}
