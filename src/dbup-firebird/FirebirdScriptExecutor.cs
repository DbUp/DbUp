using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using FirebirdSql.Data.FirebirdClient;

namespace DbUp.Firebird
{
    /// <summary>
    /// An implementation of <see cref="ScriptExecutor"/> that executes against a Firebird database.
    /// </summary>
    public class FirebirdScriptExecutor : ScriptExecutor
    {
        /// <summary>
        /// Initializes an instance of the <see cref="FirebirdScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public FirebirdScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new FirebirdObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journal)
        {
        }

        protected override string GetVerifySchemaSql(string schema)
        {
            throw new NotSupportedException();
        }

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action executeCommand)
        {
            try
            {
                executeCommand();
            }
            catch (FbException fbException)
            {
                Log().WriteInformation("Firebird exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Firebird error code: {1}; SQLSTATE {2}; Message: {3}", index, fbException.ErrorCode, fbException.SQLSTATE, fbException.Message);
                Log().WriteError(fbException.ToString());
                throw;
            }
        }
    }
}
