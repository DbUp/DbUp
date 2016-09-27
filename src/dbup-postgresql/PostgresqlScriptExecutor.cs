using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Logging;
using Npgsql;
using DbUp.Support;

namespace DbUp.Postgresql
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a Postgresql database.
    /// </summary>
    public class PostgresqlScriptExecutor : ScriptExecutor
    {
        static readonly ILog log = LogProvider.For<PostgresqlScriptExecutor>();

        /// <summary>
        /// Initializes an instance of the <see cref="PostgresqlScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journal">Database journal</param>
        public PostgresqlScriptExecutor(Func<IConnectionManager> connectionManagerFactory, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new PostgresqlObjectParser(), schema, variablesEnabled, scriptPreprocessors, journal)
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
            catch (NpgsqlException exception)
            {
                log.InfoFormat("Npgsql exception has occured in script: '{0}'", script.Name);
                log.ErrorFormat("Script block number: {0}; Block line {1}; Position: {2}; Message: {3}", index, exception.Line, exception.Position, exception.Message);
                log.Error(exception.ToString());
                throw;
            }
        }

    }
}
