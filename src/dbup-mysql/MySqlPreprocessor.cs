using DbUp.Engine;

namespace DbUp.MySql
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with MySql.
    /// </summary>
    public class MySqlPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some proprocessing step on a MySql script.
        /// </summary>
        public string Process(string contents) => contents;
    }
}
