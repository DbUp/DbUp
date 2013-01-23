using System.Collections.Generic;
using DbUp.Builder;

namespace DbUp.Engine
{
    /// <summary>
    /// This interface is implemented by classes that execute upgrade scripts against a database.
    /// </summary>
    public interface ISqlScriptExecutor
    {
        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="upgradeConfiguration"></param>
        void Execute(SqlScript script, UpgradeConfiguration upgradeConfiguration);

        /// <summary>
        /// Verifies the specified schema exists and is valid
        /// </summary>
        void VerifySchema(UpgradeConfiguration upgradeConfiguration);

        /// <summary>
        /// Timeout for each section of the script in seconds. If not set, the default timeout for the executor is used.
        /// </summary>
        int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        ///     Script Preprocessors in addition to variable substitution
        /// </summary>
        IList<ISqlScriptPreprocessor> ScriptPreprocessors { get; set; }
    }
}