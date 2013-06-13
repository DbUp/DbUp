using System;
using System.Data;

namespace DbUp.Engine
{
    /// <summary>
    /// A class which represents a script, allowing you to dynamically generate a sql script at runtime
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// Provides the Sql Script to execute
        /// </summary>
        /// <param name="dbCommandFactory">A factory to create open and active database commands</param>
        /// <returns>The Sql Script contents</returns>
        string ProvideScript(Func<IDbCommand> dbCommandFactory);
    }
}