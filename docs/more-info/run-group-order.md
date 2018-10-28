In DbUp 4.2 the concept of script types were added.  By default, DBUp will run all the scripts in alphabetical order.  This works great for run once scripts, but sometimes it might be useful to execute some run always scripts before the run once scripts occurs, and then run some follow up run always scripts.  The concept of a run group order has been added as well.

Run group order will group a set of scripts together and run those in alphabetical order.  In this example, some pre-deployment scripts are ran, then the deployment run once scripts, and then some post deployment scripts are run.

``` csharp
var upgradeEngineBuilder = DeployChanges.To
    .SqlDatabase(connectionString, null) //null or "" for default schema for user
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.PreDeployment."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 1})
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.Scripts."), new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 2})
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.PostDeployment."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 3})
    .LogToConsole();

var upgrader = upgradeEngineBuilder.Build();

var result = upgrader.PerformUpgrade();

// Display the result
if (result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Success!");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.WriteLine("Failed!");
}
```