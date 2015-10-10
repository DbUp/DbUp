﻿using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using System.Data.SqlServerCe;
using DbUp.Support;
using DbUp.Support.SqlServer;

namespace DbUp.SqlCe
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a SqlCe database.
    /// </summary>
    public class SqlCeScriptExecutor : ScriptExecutor
    {

        /// <summary>
        /// Initializes an instance of the <see cref="PostgresqlScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        public SqlCeScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors)
            : base(connectionManagerFactory, new SqlCeObjectParser(), log, schema, variablesEnabled, scriptPreprocessors)
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
                Log().WriteInformation("SqlCe exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Native Error: {1}; Message: {2}", index, exception.NativeError, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }

    }
}
