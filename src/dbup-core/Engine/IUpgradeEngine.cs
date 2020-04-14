using System;
using System.Collections.Generic;

namespace DbUp.Engine
{
    public interface IUpgradeEngine
    {
        /// <summary>
        /// An event that is raised after each script is executed.
        /// </summary>
        event EventHandler ScriptExecuted;

        /// <summary>
        /// Determines whether the database is out of date and can be upgraded.
        /// </summary>
        bool IsUpgradeRequired();

        /// <summary>
        /// Tries to connect to the database.
        /// </summary>
        /// <param name="errorMessage">Any error message encountered.</param>
        /// <returns></returns>
        bool TryConnect(
            out string errorMessage);

        /// <summary>
        /// Performs the database upgrade.
        /// </summary>
        DatabaseUpgradeResult PerformUpgrade();

        List<SqlScript> GetDiscoveredScripts();

        /// <summary>
        /// Returns a list of scripts that will be executed when the upgrade is performed
        /// </summary>
        /// <returns>The scripts to be executed</returns>
        List<SqlScript> GetScriptsToExecute();

        List<string> GetExecutedButNotDiscoveredScripts();

        List<string> GetExecutedScripts();

        ///<summary>
        /// Creates version record for any new migration scripts without executing them.
        /// Useful for bringing development environments into sync with automated environments
        ///</summary>
        ///<returns></returns>
        DatabaseUpgradeResult MarkAsExecuted();

        DatabaseUpgradeResult MarkAsExecuted(
            string latestScript);
    }
}