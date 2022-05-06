using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Google.Cloud.Spanner.Data;

namespace DbUp.Spanner
{
    /// <summary>
    /// An implementation of <see cref="ScriptExecutor"/> that executes against a Spanner database.
    /// </summary>
    public class SpannerScriptExecutor : ScriptExecutor
    {
        /// <summary>
        /// Initializes an instance of the <see cref="SpannerScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public SpannerScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journalFactory)
            : base(connectionManagerFactory, new SpannerObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journalFactory)
        {
        }

        protected override string GetVerifySchemaSql(string schema)
        {
            throw new NotSupportedException("Not implemented as named schemas are not supported by spanner.");
        }

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action executeCommand)
        {
            try
            {
                executeCommand();
            }
            catch (SpannerException exception)
            {
                var code = exception.ErrorCode;
                Log().WriteInformation("Spanner exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Spanner error code: {1}; Message: {2}", index, code, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }

    }
}
