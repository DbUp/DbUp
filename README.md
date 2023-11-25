DbUp is a set of .NET libraries that helps you to deploy changes to different databases like SQL Server. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.

# Documentation

To learn more about DbUp check out the [documentation](https://dbup.readthedocs.io/en/latest/).

# Build Status

| Package          | Stable                                                                                                                                                                                                                             | Prerelease                                                                                                                                  | Issues                                                                                                                         |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Documentation    | [![Documentation Status](https://readthedocs.org/projects/dbup/badge/?version=stable)](https://readthedocs.org/projects/dbup/?badge=stable)                                                                                        | [![Documentation Status](https://readthedocs.org/projects/dbup/badge/?version=latest)](https://readthedocs.org/projects/dbup/?badge=latest) |                                                                                                                                |
| DbUp-Core        | [![NuGet](https://img.shields.io/nuget/dt/DbUp.svg)](https://www.nuget.org/packages/dbup) [![NuGet](https://img.shields.io/nuget/v/DbUp.svg)](https://www.nuget.org/packages/dbup)                                                 | [![NuGet](https://img.shields.io/nuget/vpre/DbUp.svg)](https://www.nuget.org/packages/dbup)                                                 | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-core)](https://github.com/DbUp/DbUp/labels/dbup-core)             |
| DbUp-SqlServer   | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlserver.svg)](https://www.nuget.org/packages/dbup-sqlserver) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlserver.svg)](https://www.nuget.org/packages/dbup-sqlserver)         | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlserver.svg)](https://www.nuget.org/packages/dbup-sqlserver)                             | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-sqlserver)](https://github.com/DbUp/DbUp/labels/dbup-sqlserver)   |
| DbUp-MySql       | [![NuGet](https://img.shields.io/nuget/dt/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) [![NuGet](https://img.shields.io/nuget/v/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql)                         | [![NuGet](https://img.shields.io/nuget/vpre/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql)                                     | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-mysql)](https://github.com/DbUp/DbUp/labels/dbup-mysql)           |
| DbUp-SQLite      | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite)                     | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite)                                   | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-sqlite)](https://github.com/DbUp/DbUp/labels/dbup-sqlite)         |
| DbUp-SQLite-Mono | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono)                         | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-sqlite)](https://github.com/DbUp/DbUp/labels/dbup-sqlite)         |
| DbUp-SqlCe       | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce)                         | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce)                                     | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-sqlce)](https://github.com/DbUp/DbUp/labels/dbup-sqlce)           |
| DbUp-PostgreSQL  | [![NuGet](https://img.shields.io/nuget/dt/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) [![NuGet](https://img.shields.io/nuget/v/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql)     | [![NuGet](https://img.shields.io/nuget/vpre/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql)                           | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-postgresql)](https://github.com/DbUp/DbUp/labels/dbup-postgresql) |
| DbUp-Firebird    | [![NuGet](https://img.shields.io/nuget/dt/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) [![NuGet](https://img.shields.io/nuget/v/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird)             | [![NuGet](https://img.shields.io/nuget/vpre/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird)                               | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-firebird)](https://github.com/DbUp/DbUp/labels/dbup-firebird)     |
| DbUp-Oracle      | [![NuGet](https://img.shields.io/nuget/dt/dbup-oracle.svg)](https://www.nuget.org/packages/dbup-oracle) [![NuGet](https://img.shields.io/nuget/v/dbup-oracle.svg)](https://www.nuget.org/packages/dbup-oracle)                     | [![NuGet](https://img.shields.io/nuget/vpre/dbup-oracle.svg)](https://www.nuget.org/packages/dbup-oracle)                                   | [![view](https://img.shields.io/github/issues/DbUp/DbUp/dbup-oracle)](https://github.com/DbUp/DbUp/labels/dbup-oracle)         |

# Development

For successfull unit testing install

- SQL Server Compact 4.0 SP1 https://www.microsoft.com/en-us/download/details.aspx?id=30709

# Extensions by the community

Mainteannce and support for the extenstions where not provided by the DbUp project. If you have questions to the extions, please ask the author of the extenstion.

## DbUpX

[https://github.com/fiscaltec/DbUpX](https://github.com/fiscaltec/DbUpX)

Extensions to DbUp supporting easy filtering, ordering and versioning:

- a journaling system that stores hashes of script contents, so we know if they need to rerun,
- a concept of "dependency comments" in scripts that let you more easily control the ordering of scripts,
- protection against code reorganisation affecting long script names,
- utilities for sorting and filtering scripts in helpful ways.

# Known Issues / Quirks

## dbup-firebird

### Semi-colon Delimiter in multi statement scripts

The delimiter in a multi statement script should be on a new-line. I.e:

```sql
ALTER TABLE "MyTable" ADD "foo" int default null
;

ALTER TABLE "MyTable" ADD "bar" int default null
;
```
