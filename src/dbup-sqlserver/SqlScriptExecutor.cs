using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.SqlServer
{
    /// <summary>
    /// An implementation of <see cref="ScriptExecutor"/> that executes against a SQL Server database.
    /// </summary>
    public class SqlScriptExecutor : ScriptExecutor
    {
        /// <summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public SqlScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journalFactory)
            : base(connectionManagerFactory, new SqlServerObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journalFactory)
        {
        }

        protected override string GetVerifySchemaSql(string schema)
        {
            return string.Format(@"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA [{0}]')", Schema);
        }

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action executeCommand)
        {
            try
            {
                executeCommand();
            }
            catch (SqlException sqlException)
            {
                Log().WriteInformation("SQL exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Block line {1}; Procedure {2}; Number {3}; Message: {4}", index, sqlException.LineNumber, sqlException.Procedure, sqlException.Number, sqlException.Message);
                Log().WriteError(sqlException.ToString());
                throw;
            }
        }
    }
}
