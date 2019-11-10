using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using MySql.Data.MySqlClient;

namespace DbUp.MySql
{
    /// <summary>
    /// An implementation of <see cref="ScriptExecutor"/> that executes against a MySql database.
    /// </summary>
    public class MySqlScriptExecutor : ScriptExecutor
    {
        /// <summary>
        /// Initializes an instance of the <see cref="MySqlScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public MySqlScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journalFactory)
            : base(connectionManagerFactory, new MySqlObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journalFactory)
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
            catch (MySqlException exception)
            {
#if MY_SQL_DATA_6_9_5
                var code = exception.ErrorCode;
#else
                var code = exception.Code;
#endif
                Log().WriteInformation("MySql exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; MySql error code: {1}; Number {2}; Message: {3}", index, code, exception.Number, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }

    }
}
