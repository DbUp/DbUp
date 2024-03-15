using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;

namespace DbUp.Engine;

/// <summary>
/// This class orchestrates the database upgrade process.
/// </summary>
public class UpgradeEngine
{
    protected readonly UpgradeConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeEngine"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public UpgradeEngine(UpgradeConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <summary>
    /// An event that is raised after each script is executed.
    /// </summary>
    public event EventHandler ScriptExecuted;

    /// <summary>
    /// Invokes the <see cref="ScriptExecuted"/> event; called whenever a script is executed.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnScriptExecuted(ScriptExecutedEventArgs e)
    {
        ScriptExecuted?.Invoke(this, e);
    }

    /// <summary>
    /// Determines whether the database is out of date and can be upgraded.
    /// </summary>
    public virtual bool IsUpgradeRequired()
    {
        return GetScriptsToExecute().Count() != 0;
    }

    /// <summary>
    /// Tries to connect to the database.
    /// </summary>
    /// <param name="errorMessage">Any error message encountered.</param>
    /// <returns></returns>
    public virtual bool TryConnect(out string errorMessage)
    {
        return configuration.ConnectionManager.TryConnect(configuration.Log, out errorMessage);
    }

    /// <summary>
    /// Performs the database upgrade.
    /// </summary>
    public virtual DatabaseUpgradeResult PerformUpgrade()
    {
        var executed = new List<SqlScript>();

        SqlScript executedScript = null;
        try
        {
            using (configuration.ConnectionManager.OperationStarting(configuration.Log, executed))
            {
                configuration.Log.LogInformation("Beginning database upgrade");

                var scriptsToExecute = GetScriptsToExecuteInsideOperation();

                if (scriptsToExecute.Count == 0)
                {
                    configuration.Log.LogInformation("No new scripts need to be executed - completing.");
                    return new DatabaseUpgradeResult(executed, true, null, null);
                }

                configuration.ScriptExecutor.VerifySchema();

                foreach (var script in scriptsToExecute)
                {
                    executedScript = script;

                    configuration.ScriptExecutor.Execute(script, configuration.Variables);

                    OnScriptExecuted(new ScriptExecutedEventArgs(script, configuration.ConnectionManager));

                    executed.Add(script);
                }

                configuration.Log.LogInformation("Upgrade successful");
                return new DatabaseUpgradeResult(executed, true, null, null);
            }
        }
        catch (Exception ex)
        {
            if (executedScript != null)
            {
                ex.Data["Error occurred in script: "] = executedScript.Name;
            }

            configuration.Log.LogError(ex, "Upgrade failed due to an unexpected exception: {0}", ex.ToString());
            return new DatabaseUpgradeResult(executed, false, ex, executedScript);
        }
    }

    /// <summary>
    /// Returns a list of scripts that will be executed when the upgrade is performed
    /// </summary>
    /// <returns>The scripts to be executed</returns>
    public virtual List<SqlScript> GetScriptsToExecute()
    {
        using (configuration.ConnectionManager.OperationStarting(configuration.Log, new List<SqlScript>()))
        {
            return GetScriptsToExecuteInsideOperation();
        }
    }

    public virtual List<string> GetExecutedButNotDiscoveredScripts()
    {
        return GetExecutedScripts().Except(GetDiscoveredScriptsAsEnumerable().Select(x => x.Name)).ToList();
    }

    public virtual List<SqlScript> GetDiscoveredScripts()
    {
        return GetDiscoveredScriptsAsEnumerable().ToList();
    }

    IEnumerable<SqlScript> GetDiscoveredScriptsAsEnumerable()
    {
        return configuration.ScriptProviders.SelectMany(scriptProvider => scriptProvider.GetScripts(configuration.ConnectionManager));
    }

    List<SqlScript> GetScriptsToExecuteInsideOperation()
    {
        var allScripts = GetDiscoveredScriptsAsEnumerable();
        var executedScriptNames = new HashSet<string>(configuration.Journal.GetExecutedScripts());

        var sorted = allScripts.OrderBy(s => s.SqlScriptOptions.RunGroupOrder).ThenBy(s => s.Name, configuration.ScriptNameComparer);
        var filtered = configuration.ScriptFilter.Filter(sorted, executedScriptNames, configuration.ScriptNameComparer);
        return filtered.ToList();
    }

    public virtual List<string> GetExecutedScripts()
    {
        using (configuration.ConnectionManager.OperationStarting(configuration.Log, new List<SqlScript>()))
        {
            return configuration.Journal.GetExecutedScripts()
                .ToList();
        }
    }

    ///<summary>
    /// Creates version record for any new migration scripts without executing them.
    /// Useful for bringing development environments into sync with automated environments
    ///</summary>
    ///<returns></returns>
    public virtual DatabaseUpgradeResult MarkAsExecuted()
    {
        var marked = new List<SqlScript>();
        SqlScript executedScript = null;
        using (configuration.ConnectionManager.OperationStarting(configuration.Log, marked))
        {
            try
            {
                var scriptsToExecute = GetScriptsToExecuteInsideOperation();

                foreach (var script in scriptsToExecute)
                {
                    executedScript = script;
                    configuration.ConnectionManager.ExecuteCommandsWithManagedConnection(
                        connectionFactory => configuration.Journal.StoreExecutedScript(script, connectionFactory));
                    configuration.Log.LogInformation("Marking script {0} as executed.", script.Name);
                    marked.Add(script);
                }

                configuration.Log.LogInformation("Script marking successful.");
                return new DatabaseUpgradeResult(marked, true, null, null);
            }
            catch (Exception ex)
            {
                configuration.Log.LogError(ex, "Upgrade failed due to an unexpected exception: {0}", ex.ToString());
                return new DatabaseUpgradeResult(marked, false, ex, executedScript);
            }
        }
    }

    public virtual DatabaseUpgradeResult MarkAsExecuted(string latestScript)
    {
        var marked = new List<SqlScript>();
        SqlScript executedScript = null;
        using (configuration.ConnectionManager.OperationStarting(configuration.Log, marked))
        {
            try
            {
                var scriptsToExecute = GetScriptsToExecuteInsideOperation();

                foreach (var script in scriptsToExecute)
                {
                    executedScript = script;
                    configuration.ConnectionManager.ExecuteCommandsWithManagedConnection(
                        commandFactory => configuration.Journal.StoreExecutedScript(script, commandFactory));
                    configuration.Log.LogInformation("Marking script {0} as executed.", script.Name);
                    marked.Add(script);
                    if (script.Name.Equals(latestScript))
                    {
                        break;
                    }
                }

                configuration.Log.LogInformation("Script marking successful.");
                return new DatabaseUpgradeResult(marked, true, null, null);
            }
            catch (Exception ex)
            {
                configuration.Log.LogError(ex, "Upgrade failed due to an unexpected exception: {0}", ex.ToString());
                return new DatabaseUpgradeResult(marked, false, ex, executedScript);
            }
        }
    }
}
