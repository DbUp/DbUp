# Using DbUp
DbUp uses a builder to configure your database deployments, you then build the upgrade engine and run your database migrations.

## Configuration
The entrypoint is `DeployChanges.To`. There are then extension methods for all of the [supported databases](./supported-databases.md).

Then you can configure:

* [Journaling](./journaling.md)
* [Script Providers](./script-providers.md)
* [Logging](./logging.md)
* [Variable Substitution](./variable-substitution.md)
* [Transaction Usage](./transactions.md)
* [Preprocessors](./preprocessors.md)

## Deploying
Once you have configured DbUp

``` csharp
var upgradeEngine = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();
```

You can:
* Get scripts which will be executed (`GetScriptsToExecute()`)
* Get already executed scripts (`GetExecutedScripts()`)
* Check if an upgrade is required (`IsUpgradeRequired()`)
* Creates version record for any new migration scripts without executing them (`MarkAsExecuted`)
  * Useful for bringing development environments into sync with automated environments
* Try to connect to the database (`TryConnect()`)
* Perform the database upgrade (`PerformUpgrade()`)
