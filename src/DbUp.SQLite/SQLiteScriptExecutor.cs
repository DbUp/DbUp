﻿using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

#if MONO
using SQLiteException = Mono.Data.Sqlite.SqliteException;
#else
using System.Data.SQLite;
#endif


namespace DbUp.SQLite
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a SQLite database.
    /// </summary>
    public class SQLiteScriptExecutor : ScriptExecutor
    {

        /// <summary>
        /// Initializes an instance of the <see cref="SQLiteScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journal">Database journal</param>
        public SQLiteScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new SQLiteObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journal)
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
            catch (SQLiteException exception)
            {
                Log().WriteInformation("SQLite exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Error Code: {1}; Message: {2}", index, exception.ErrorCode, exception.Message);
                Log().WriteError(exception.ToString());
                throw;
            }
        }

    }
}
