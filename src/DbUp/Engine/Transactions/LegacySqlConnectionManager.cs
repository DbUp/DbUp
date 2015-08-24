using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Support.SqlServer;

namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// Allows backwards compatibility with previous API/behaviour of using connection factories with DbUp
    /// </summary>
    public class LegacySqlConnectionManager : IConnectionManager
    {
        private readonly Func<IDbConnection> connectionFactory;

        /// <summary>
        /// Ctor for LegacySqlConnectionManager
        /// </summary>
        /// <param name="connectionFactory">The connectionFactory</param>
        public LegacySqlConnectionManager(Func<IDbConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public IDisposable OperationStarting(IUpgradeLog upgradeLog, List<SqlScript> executedScripts)
        {
            return new DoNothingDisposible();
        }

        public void ExecuteCommandsWithManagedConnection(Action<Func<IDbCommand>> action)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                action(()=>connection.CreateCommand());
            }
        }

        public T ExecuteCommandsWithManagedConnection<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return actionWithResult(()=>connection.CreateCommand());
            }
        }

        public TransactionMode TransactionMode { get; set; }

        public bool IsScriptOutputLogged { get; set; }

        public IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;           
        }

        class DoNothingDisposible : IDisposable
        {
            public void Dispose()
            {

            }
        }
    }
}