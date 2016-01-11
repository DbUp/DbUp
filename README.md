DbUp is a .NET library that helps you to deploy changes to SQL Server databases. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.

[![Join the chat at https://gitter.im/DbUp/DbUp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/DbUp/DbUp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/vm3lg8kk1pxn64pj/branch/master?svg=true)](https://ci.appveyor.com/project/rintje/dbup/branch/master)

|                  | Stable | Prerelease |
| :--:             |  :--:  |    :--:    |
| Documentation    | [![Documentation Status](https://readthedocs.org/projects/dbup/badge/?version=stable)](https://readthedocs.org/projects/dbup/?badge=stable) | [![Documentation Status](https://readthedocs.org/projects/dbup/badge/?version=latest)](https://readthedocs.org/projects/dbup/?badge=latest) |
| DbUp             | [![NuGet](https://img.shields.io/nuget/dt/DbUp.svg)](https://www.nuget.org/packages/dbup) [![NuGet](https://img.shields.io/nuget/v/DbUp.svg)](https://www.nuget.org/packages/dbup) | [![NuGet](https://img.shields.io/nuget/vpre/DbUp.svg)](https://www.nuget.org/packages/dbup) |
| DbUp-MySql       | [![NuGet](https://img.shields.io/nuget/dt/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) [![NuGet](https://img.shields.io/nuget/v/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) |
| DbUp-SQLite      | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) |
| DbUp-SQLite-Mono | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) |
| DbUp-SqlCe       | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce) |
| DbUp-PostgreSQL  | [![NuGet](https://img.shields.io/nuget/dt/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) [![NuGet](https://img.shields.io/nuget/v/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) |
| DbUp-Firebird  | [![NuGet](https://img.shields.io/nuget/dt/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) [![NuGet](https://img.shields.io/nuget/v/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) |

# Fork specific info

For use in Octopus Deploy we created an additional, specialized file system provider to improve support for our build scripts folder structure. 
DbUp is a very useful project to us, so i hope i can help providing some extra value here. Feel free to cherry pick. Al of these customizations (in master) were submitted to upstream as a pull request (https://github.com/DbUp/DbUp/pull/148).

The new provider is named VersionedFolderScriptProvider and adds the following features:

### Read scripts from a folder containing one subfolder per version
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
### Specify a target version
Use semantic versioning to exclude scripts from folders from versions newer than the target version. This makes it possible to use the same scriptbase for patch and release deploys. To make this possible, the foldersnames must be parseable to a version (i.e. contains 1 - 4 delimited decimals),

In the example above, a target version of '1.0.1' will result in the following ordered set of scripts:
```
1.0\1_apple.sql
1.0\2_banana.sql
1.0.1\1_apple.sql
1.0.1\2_pear.sql
```
### Misc.
##### The script provider filter is applied twice, in consequtive order:
1. To the folder name
2. To the effective scriptname

##### Unit tests are added

## Powershell example
```
$databaseName = $args[0]
$databaseServer = $args[1]
$scriptRootFolder = $args[2]
$targetVersion = $args[3]

Add-Type -Path "C:\Development resources\Code\Github\DbUp\src\DbUp\bin\Debug\DbUp.dll"

function FilterFunc ($a,$b) {
  return `
	-not $a.ToLower().Contains("obsolete") -and `
	-not $a.ToLower().StartsWith("branches") -and `
	-not $a.ToLower().StartsWith("customerspecific") -and `
	-not $a.ToLower().StartsWith("warehouse");
}

$dbUp = [DbUp.DeployChanges]::To
$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, "server=$databaseServer;database=$databaseName;Trusted_Connection=Yes;Connection Timeout=120;")
$dbUp = [StandardExtensions]::WithScriptsFromVersionFolders($dbUp, $scriptRootFolder, ${function:FilterFunc}, $targetVersion)
$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'dbo', 'SchemaVersions')
$dbUp = [StandardExtensions]::LogToConsole($dbUp)
$upgradeResult = $dbUp.Build().PerformUpgrade()
```


