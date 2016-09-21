using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Logging;
using FirebirdSql.Data.FirebirdClient;
using DbUp.Support;

namespace DbUp.Firebird
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a Firebird database.
    /// </summary>
    public class FirebirdScriptExecutor : ScriptExecutor
    {
        static readonly ILog log = LogProvider.For<FirebirdScriptExecutor>();

        /// <summary>
        /// Initializes an instance of the <see cref="FirebirdScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journal">Database journal</param>
        public FirebirdScriptExecutor(Func<IConnectionManager> connectionManagerFactory, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new FirebirdObjectParser(), schema, variablesEnabled, scriptPreprocessors, journal)
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
            catch (FbException fbException)
            {
                log.InfoFormat("Firebird exception has occured in script: '{0}'", script.Name);
                log.ErrorException("Firebird error code: {0}; SQLSTATE {1}; Message: {2}", fbException, index, fbException.ErrorCode, fbException.SQLSTATE, fbException.Message);
                throw;
            }
        }

    }
}
