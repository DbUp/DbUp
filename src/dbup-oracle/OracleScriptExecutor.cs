using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Oracle.ManagedDataAccess.Client;

namespace DbUp.Oracle
{
    public class OracleScriptExecutor : ScriptExecutor
    {
        /// <summary>
        /// Initializes an instance of the <see cref="OracleScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public OracleScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journalFactory)
            : base(connectionManagerFactory, new OracleObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journalFactory)
        {

        }

        protected override string GetVerifySchemaSql(string schema)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public override void Execute(SqlScript script)
        {
            Execute(script, null);
        }

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action executeCommand)
        {
            try
            {
                executeCommand();
            }
            catch (OracleException exception)
            {
#if MY_SQL_DATA_6_9_5
                var code = exception.ErrorCode;
#else
                var code = exception.ErrorCode;
#endif
                Log().WriteInformation("Oracle exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Oracle error code: {0}; Number {1}; Message: {2}", index, code, exception.Number, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }
    }
}
