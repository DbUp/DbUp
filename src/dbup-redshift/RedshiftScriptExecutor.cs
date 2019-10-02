using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Npgsql;

namespace DbUp.Redshift
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a Redshift database.
    /// </summary>
    public class RedshiftScriptExecutor : ScriptExecutor
    {

        /// <summary>
        /// Initializes an instance of the <see cref="RedshiftScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public RedshiftScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journalFactory)
            : base(connectionManagerFactory, new RedshiftObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journalFactory)
        {

        }

        protected override string GetVerifySchemaSql(string schema)
           => $@"CREATE SCHEMA IF NOT EXISTS {schema}";


        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action excuteCommand)
        {
            try
            {
                excuteCommand();
            }
#if NPGSQLv2
            catch (NpgsqlException exception)
#else
            catch (PostgresException exception)
#endif
            {
                Log().WriteInformation("Npgsql exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Block line {1}; Position: {2}; Message: {3}", index, exception.Line, exception.Position, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }

    }
}
