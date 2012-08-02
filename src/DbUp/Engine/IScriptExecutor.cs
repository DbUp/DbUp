using System.Collections.Generic;

namespace DbUp.Engine
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
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="variables">Variables to replace in the script</param>
        void Execute(SqlScript script, IDictionary<string, string> variables);

        /// <summary>
        /// Verifies the specified schema exists and is valid
        /// </summary>
        void VerifySchema();

        /// <summary>
        /// Timeout for each section of the script in seconds. If not set, the default timeout for the executor is used.
        /// </summary>
        int? ExecutionTimeoutSeconds { get; set; }
    }
}