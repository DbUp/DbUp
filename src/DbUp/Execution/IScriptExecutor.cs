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
        void Execute(SqlScript script);

        /// <summary>
        /// Verifies the specified schema exists and is valid
        /// </summary>
        void VerifySchema();
    }
}