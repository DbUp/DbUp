using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using MySql.Data.MySqlClient;
using DbUp.Support;
using DbUp.Support.SqlServer;

namespace DbUp.MySql
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a MySql database.
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
        /// <param name="journal">Database journal</param>
        public MySqlScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new MySqlObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journal)
        {

        }

        protected override string GetVerifySchemaSql(string schema)
        {
            throw new NotSupportedException();
        }

     

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action excuteCommand)
        {
            try
            {
                excuteCommand();
            }
            catch (MySqlException exception)
            {
                Log().WriteInformation("MySql exception has occured in script: '{0}'", script.Name);
                Log().WriteError("MySql error code: {0}; Number {1}; Message: {2}", index, exception.ErrorCode, exception.Number, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }

    }
}
