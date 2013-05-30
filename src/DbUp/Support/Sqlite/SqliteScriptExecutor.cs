using System;
using System.Data;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;

namespace DbUp.Support.Sqlite
{
    /// <summary>
    /// A standard implementation of the IScriptExecutor interface that executes against a SQLite database
    /// database.
    /// </summary>
    public sealed class SQLiteScriptExecutor : IScriptExecutor
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly Func<bool> variablesEnabled;

        /// <summary>
        /// SQLCommand Timeout in seconds. If not set, the default SQLCommand timeout is not changed.
        /// </summary>
        public int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="SQLiteScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="log"></param>
        /// <param name="variablesEnabled"></param>
        /// <param name="scriptPreprocessors"></param>
        public SQLiteScriptExecutor(Func<IDbConnection> connectionFactory, Func<IUpgradeLog> log, Func<bool> variablesEnabled, IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            this.connectionFactory = connectionFactory;
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
        }

        /// <summary>
        /// Sqlite does support schema and always verified.
        /// </summary>
        public void VerifySchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void Execute(SqlScript script)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="variables">Variables to replace in the script</param>
        public void Execute(SqlScript script, IDictionary<string, string> variables)
        {
            throw new NotImplementedException();
        }
    }
}
