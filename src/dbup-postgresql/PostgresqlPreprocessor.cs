using DbUp.Engine;

namespace DbUp.Postgresql
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with PostgreSQL.
    /// </summary>
    public class PostgresqlPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some preprocessing step on a PostgreSQL script.
        /// </summary>
        public string Process(string contents) => contents;
    }
}
