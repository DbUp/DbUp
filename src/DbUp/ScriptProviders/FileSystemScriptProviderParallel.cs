using System;
using System.Collections.Concurrent;
using System.IO;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;


namespace DbUp.ScriptProviders
{
	///<summary>
	/// Alternate <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts via a directory in a ThreadSafe manner
	///</summary>
	public class FileSystemScriptProviderParallel : FileSystemScriptProvider
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="directoryPath">Path to SQL upgrade scripts</param>
		public FileSystemScriptProviderParallel(string directoryPath) : base(directoryPath)
		{

		}

		/// <summary>
		/// Gets all scripts that should be executed.
		/// </summary>
		public new ConcurrentBag<SqlScript> GetScripts(IConnectionManager connectionManager)
		{
			return new ConcurrentBag<SqlScript>(base.GetScripts(null));
		}

	}
}