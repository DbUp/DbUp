using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using FirebirdSql.Data.FirebirdClient;
using DbUp.Support;
using DbUp.Support.SqlServer;

namespace DbUp.Firebird
{
    /// <summary>
    /// An implementation of ScriptExecutor that executes against a Firebird database.
    /// </summary>
    public class FirebirdScriptExecutor : ScriptExecutor
    {

        /// <summary>
        /// Initializes an instance of the <see cref="FirebirdScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        public FirebirdScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors)
            : base(connectionManagerFactory, log, schema, variablesEnabled, scriptPreprocessors)
        {

        }

        protected override string GetVerifySchemaSql(string schema)
        {
            throw new NotSupportedException();
        }

        protected override string QuoteSqlObjectName(string objectName)
        {
            // Firebird appears to have allways used the SQL Server implementation of this..
            // Need to investiagte to see if it needs its own implementation of object quoting.
            return SqlObjectParser.QuoteSqlObjectName(objectName, ObjectNameOptions.Trim);
        }    

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action excuteCommand)
        {
            try
            {
                excuteCommand();
            }
            catch (FbException fbException)
            {
                Log().WriteInformation("Firebird exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Firebird error code: {0}; SQLSTATE {1}; Message: {2}", index, fbException.ErrorCode, fbException.SQLSTATE, fbException.Message);
                Log().WriteError(fbException.ToString());
                throw;
            }
        }

    }
}
