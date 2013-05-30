using DbUp.Engine;

namespace DbUp.Sqlite
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with Sqlite
    /// </summary>
    public class SQLitePreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some proprocessing step on a SQLite script
        /// </summary>
        public string Process(string contents)
        {
            throw new System.NotImplementedException();
        }
    }
}
