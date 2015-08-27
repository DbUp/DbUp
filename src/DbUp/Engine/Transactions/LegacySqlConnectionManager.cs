using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Support.SqlServer;

namespace DbUp.Engine.Transactions
{
	/// <summary>
	///    Allows backwards compatibility with previous API/behaviour of using connection factories with DbUp
	/// </summary>
	public class LegacySqlConnectionManager : IConnectionManager
	{
		private readonly Func<IDbConnection> connectionFactory;
		public bool IsScriptOutputLogged { get; set; }
		public TransactionMode TransactionMode { get; set; }

		/// <summary>
		///    Ctor for LegacySqlConnectionManager
		/// </summary>
		/// <param name="connectionFactory">The connectionFactory</param>
		public LegacySqlConnectionManager (Func<IDbConnection> connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public void ExecuteCommandsWithManagedConnection (Action<Func<IDbCommand>> action)
		{
			using (var connection = connectionFactory ()) {
				connection.Open ();
				action (() => connection.CreateCommand ());
			}
		}

		public T ExecuteCommandsWithManagedConnection<T> (Func<Func<IDbCommand>, T> actionWithResult)
		{
			using (var connection = connectionFactory ()) {
				connection.Open ();
				return actionWithResult (() => connection.CreateCommand ());
			}
		}

		public IDisposable OperationStarting (IUpgradeLog upgradeLog, List<SqlScript> executedScripts)
		{
			return new DoNothingDisposible ();
		}

		public IEnumerable<string> SplitScriptIntoCommands (string scriptContents)
		{
			var commandSplitter = new SqlCommandSplitter ();
			var scriptStatements = commandSplitter.SplitScriptIntoCommands (scriptContents);
			return scriptStatements;
		}

		/// <summary>
		///    Tries to connect to the database.
		/// </summary>
		public bool TryConnect (IUpgradeLog upgradeLog, out string errorMessage)
		{
			try {
				errorMessage = "";
				ExecuteCommandsWithManagedConnection (dbCommandFactory => {
					                                      using (var command = dbCommandFactory ()) {
						                                      command.CommandText = "select 1";
						                                      command.ExecuteScalar ();
					                                      }
				                                      });
				return true;
			} catch (Exception ex) {
				errorMessage = ex.Message;
				return false;
			}
		}

		#region Nested type: DoNothingDisposible

		private class DoNothingDisposible : IDisposable
		{
			public void Dispose () {}
		}

		#endregion
	}
}