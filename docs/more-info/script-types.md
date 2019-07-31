With DBUp, by default every script will only be run once.  The list of scripts ran on the database is kept in a table to ensure a script is never run again.  

There are cases where it makes sense to have a script always run during a deployment.  Some examples might include:
- Ensuring a role always exists and that role is assigned appropriate permissions
- Populating the database with initialization data
- Ensuring users are always assigned to a particular role
- Refreshing views to handle updated schema changes

Script types have been added version 4.2.  The script types currently supported are:

- RunOnce - Default, runs the script once on the database
- RunAlways - Will always run the script on the database

*Please Note:* It is important you write your run always scripts in a way in which they can always be run.  This means adding in logic gates to check to see if something already exists.  For example:

```sql
IF DATABASE_PRINCIPAL_ID('Testing_Role') IS NULL
BEGIN
	CREATE ROLE Testing_Role
end
```

You specify the script type when adding the scripts.

```csharp
var upgradeEngineBuilder = DeployChanges.To
                            .SqlDatabase(connectionString, null) //null or "" for default schema for user               
                            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), new SqlScriptOptions { ScriptType = ScriptType.RunAlways })
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

If you want to organize your scripts into multiple folders so the RunOnce scripts are grouped together and the RunAlways scripts are grouped together you could do this.  The scripts will be ran alphabetically.

```csharp
var upgradeEngineBuilder = DeployChanges.To
    .SqlDatabase(connectionString, null) //null or "" for default schema for user
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.Scripts."), new SqlScriptOptions { ScriptType = ScriptType.RunOnce })
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.RunAlways."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways })
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
