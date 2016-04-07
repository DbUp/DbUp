DbUp can get it's scripts which need to be executed from anywhere. Out of the box it has support for:

## EmbeddedScriptProvider
Finds scripts embedded in a single Assembly
### Usage
`builder.WithScriptsEmbeddedInAssembly(Assembly, [optional filter])`
### Example
`builder.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (string s) => s.StartsWith("Script"))`

## EmbeddedScriptsProvider
Finds scripts embedded in one or more assemblies
### Usage
`builder.WithScriptsEmbeddedInAssemblies(Assembly[], [optional filter])`
### Example

```
builder.WithScriptsEmbeddedInAssemblies(new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(Something).Assembly
},
(string s) => s.StartsWith("Script"))
```

## StaticScriptProvider
Allows you to easily programatically supply scripts from code

### Usage
``` csharp
// Single script
builder.WithScript("name.sql", "content");
// Many scripts
builder.WithScripts(new[]
{
  new SqlScript("script1.sql", "content"),
  new SqlScript("script2.sql", "content2")
});
// Custom script provider
builder.WithScripts(new MyCustomScriptProvider());
```

## EmbeddedScriptAndCodeProvider
An enhanced script provider implementation which retrieves upgrade scripts or IScript code upgrade scripts embedded in an assembly.

**Warning:** DbUp does not protect against Sql Injection attacks, code scripts allow you to generate an upgrade script dynamically based on data in your database if you need to. If this data is put into the resulting script it could well contain a sql injection attack.

###Usage
`builder.WithScriptsAndCodeEmbeddedInAssembly(Assembly)`

## FileSystemScriptProvider
Finds scripts in a specified directory
### Usage
`builder.WithScriptsFromFileSystem(path)`

## VersionFoldersScriptProvider
Finds scripts in version subfolders in a specified directory.

### Reading scripts from a folder containing one subfolder per version
Example folder structure:
```
my-script-root-folder\
   1.0\
        1_apple.sql
        2_banana.sql
   1.0.1\
        1_apple.sql
        2_pear.sql
   2.0\
        apple.sql
        banana.sql
```

The found scripts are flattened to one list by combining the relative folder name with the script name. In this example, the resulting ordered set of scripts of will be:
```
1.0\1_apple.sql
1.0\2_banana.sql
1.0.1\1_apple.sql
1.0.1\2_pear.sql
2.0\apple.sql
2.0\banana.sql
```

### Optional: specify a target version
Use semantic versioning to exclude scripts from folders for versions newer than the target version. This makes it possible to use the same script base for patch and release deploys. To qualify, the names of all non-excluded subfolders must be parseable to a version (1 to 4 decimals delimited by dots)

In the example above, a target version of '1.0.1' will result in the following ordered set of scripts:
```
1.0\1_apple.sql
1.0\2_banana.sql
1.0.1\1_apple.sql
1.0.1\2_pear.sql
```

### Optional: specify a filter function
During search, the script provider filter is applied twice:
1. To the subfolder name
2. To the effective scriptname, e.g. subfolder\script.sql

### Usage 
`builder.WithScriptsFromVersionFolders(path, targetVersion)`

### Example 
From Powershell:

```
$databaseName = $args[0]
$databaseServer = $args[1]
$scriptRootFolder = $args[2]
$targetVersion = $args[3]

Add-Type -Path "drive:\path\to\DbUp.dll"

function FilterFunc ($a,$b) {
  return `
	-not $a.ToLower().Contains("obsolete") -and `
	-not $a.ToLower().StartsWith("branches") -and `
	-not $a.ToLower().StartsWith("customerspecific") -and `
	-not $a.ToLower().StartsWith("1.4.6-broken");
}

$dbUp = [DbUp.DeployChanges]::To
$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, "server=$databaseServer;database=$databaseName;Trusted_Connection=Yes;Connection Timeout=120;")
$dbUp = [StandardExtensions]::WithScriptsFromVersionFolders($dbUp, $scriptRootFolder, ${function:FilterFunc}, $targetVersion)
$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'dbo', 'SchemaVersions')
$dbUp = [StandardExtensions]::LogToConsole($dbUp)
$upgradeResult = $dbUp.Build().PerformUpgrade()
```

