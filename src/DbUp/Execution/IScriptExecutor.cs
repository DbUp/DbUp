using DbUp.ScriptProviders;

namespace DbUp.Execution
{
    /// <summary>
    /// This interface is implemented by classes that execute upgrade scripts against a database.
    /// </summary>
    public interface IScriptExecutor
    {
        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="log">The log.</param>
        void Execute(SqlScript script, ILog log);
    }
}