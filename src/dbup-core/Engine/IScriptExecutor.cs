using System;
using System.Collections.Generic;
using DbUp.Engine.Transactions;

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
        /// <param name="transactionMode">The transaction mode.</param>
        /// <param name="deploymentId">The deployment identifier.</param>
        void Execute(SqlScript script, TransactionMode transactionMode, Guid deploymentId);

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="transactionMode">The transaction mode.</param>
        /// <param name="variables">Variables to replace in the script</param>
        /// <param name="deploymentId">The deployment identifier.</param>
        void Execute(SqlScript script, TransactionMode transactionMode, IDictionary<string, string> variables, Guid deploymentId);

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