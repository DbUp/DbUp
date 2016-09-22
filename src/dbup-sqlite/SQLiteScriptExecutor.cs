using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Logging;
using DbUp.Support;

#if MONO
using SQLiteException = Mono.Data.Sqlite.SqliteException;
#elif NETCORE
using SQLiteException = Microsoft.Data.Sqlite.SqliteException;
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
        static readonly ILog log = LogProvider.For<SQLiteScriptExecutor>();

        /// <summary>
        /// Initializes an instance of the <see cref="SQLiteScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journal">Database journal</param>
        public SQLiteScriptExecutor(Func<IConnectionManager> connectionManagerFactory, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journal)
            : base(connectionManagerFactory, new SQLiteObjectParser(), schema, variablesEnabled, scriptPreprocessors, journal)
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
                log.InfoFormat("SQLite exception has occured in script: '{0}'", script.Name);
#if NETCORE
                log.ErrorFormat("Script block number: {0}; Error Code: {1}; Message: {2}", index, exception.SqliteErrorCode, exception.Message);
#else
                log.ErrorFormat("Script block number: {0}; Error Code: {1}; Message: {2}", index, exception.ErrorCode, exception.Message);
#endif
                log.Error(exception.ToString());
                throw;
            }
        }

    }
}
