DbUp uses a builder to configure your database deployments, you then build the upgrade engine and run your database migrations.

## Configuration
The entrypoint is `DeployChanges.To`. There are then extension methods for all of the [supported databases](./supported-databases.md).

Then you can configure:

* [Journaling](./more-info/journaling.md)
* [Script Providers](./more-info/script-providers.md)
* [Logging](./more-info/logging.md)
* [Variable Substitution](./more-info/variable-substitution.md)
* [Transaction Usage](./more-info/transactions.md)
* [Preprocessors](./more-info/preprocessors.md)

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
* Log script output (`LogScriptOutput()`)

## Hosting options
A console application is not the only way to use DbUp. For example...

* A website can detect if it needs a database upgrade when it starts. If an upgrade is required it can put itself into maintainance mode and require an administrator to login and click a button to perform the migration.

There are any number of other ways to use DbUp. Feel free to submit a pull request to update this section with more information.

### From PowerShell
Another option is call DbUp directly from PowerShell, which is useful when using DbUp from a deployment tool like Octopus Deploy.

``` PowerShell
$databaseName = $args[0]
$databaseServer = $args[1]
$scriptPath = $args[2]

Add-Type -Path (Join-Path -Path $currentPath -ChildPath 'x:\location\of\DbUp.dll')

$dbUp = [DbUp.DeployChanges]::To
$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, "server=$databaseServer;database=$databaseName;Trusted_Connection=Yes;Connection Timeout=120;")
$dbUp = [StandardExtensions]::WithScriptsFromFileSystem($dbUp, $scriptPath)
$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'MySchema', 'MyTable')
$dbUp = [StandardExtensions]::LogToConsole($dbUp)
$upgradeResult = $dbUp.Build().PerformUpgrade()
```

## Code-based scripts
Sometimes migrations may require more logic than is easy or possible to perform in SQL alone. Code-based scripts provide the facility to generate SQL in code, with an open database connection and a `System.Data.IDbCommand` factory provided.

The code-based migration is a class that implements the `IScript` interface. The `ProvideScript()` method is called when it is the migration's turn to be executed, so the scripts before it have already been executed.

This example shows a query being called and the results used to build up an arbitrary set of `INSERT` statements:

``` csharp
public class Script0005ComplexUpdate : IScript
{
    public string ProvideScript(Func<IDbCommand> commandFactory)
    {
        var cmd = commandFactory();
        cmd.CommandText = "Select * from SomeTable";
        var scriptBuilder = new StringBuilder();

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                scriptBuilder.AppendLine(string.Format("insert into AnotherTable values ({0})", reader.GetString(0)));
            }
        }

        return scriptBuilder.ToString();
    }
}
```

Of course, the command factory can be used for more than just queries. The entire migration itself can be performed in code:

``` csharp
public class Script0006UpdateInCode : IScript
{
    public string ProvideScript(Func<IDbCommand> commandFactory)
    {
        var command = commandFactory();

        command.CommandText = "CREATE TABLE [dbo].[Foo]( [Name] NVARCHAR(MAX) NOT NULL )";
        command.ExecuteNonQuery();

        return "";
    }
}
```

**WARNING:** This code is vulnerable to SQL injection attacks, this functionality is provided for flexibility but like any advanced feature use caution and make sure the data you are reading cannot contain SQL injection code.

See [script providers](./more-info/script-providers.md) for information on how to discover code scripts
