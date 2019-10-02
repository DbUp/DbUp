using DbUp.Engine;

namespace DbUp.MySql
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with MySql.
    /// </summary>
    public class MySqlPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some preprocessing step on a PostgreSQL script.
        /// </summary>
        public string Process(string contents) => contents;
    }
}
