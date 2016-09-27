using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using System.Data.SqlServerCe;
using DbUp.Logging;
using DbUp.Support;

namespace DbUp.SqlCe
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a SqlCe database.
    /// </summary>
    public class SqlCeScriptExecutor : ScriptExecutor
    {
        static readonly ILog log = LogProvider.For<SqlCeScriptExecutor>();

        /// <summary>
        /// Initializes an instance of the <see cref="SqlCeScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journal">Database journal</param>
        public SqlCeScriptExecutor(Func<IConnectionManager> connectionManagerFactory, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new SqlCeObjectParser(), schema, variablesEnabled, scriptPreprocessors, journal)
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
            catch (SqlCeException exception)
            {
                log.InfoFormat("SqlCe exception has occured in script: '{0}'", script.Name);
                log.ErrorFormat("Script block number: {0}; Native Error: {1}; Message: {2}", index, exception.NativeError, exception.Message);
                log.Error(exception.ToString());
                throw;
            }
        }

    }
}
