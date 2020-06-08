DbUp is a .NET library that helps you to deploy changes to SQL Server databases. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.

[![Join the chat at https://gitter.im/DbUp/DbUp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/DbUp/DbUp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/vm3lg8kk1pxn64pj/branch/master?svg=true)](https://ci.appveyor.com/project/DbUp/dbup/branch/master)

|                  | Stable | Prerelease |
| :--:             |  :--:  |    :--:    |
| Documentation    | [![Documentation Status](https://readthedocs.org/projects/dbup/badge/?version=stable)](https://readthedocs.org/projects/dbup/?badge=stable) | [![Documentation Status](https://readthedocs.org/projects/dbup/badge/?version=latest)](https://readthedocs.org/projects/dbup/?badge=latest) |
| DbUp             | [![NuGet](https://img.shields.io/nuget/dt/DbUp.svg)](https://www.nuget.org/packages/dbup) [![NuGet](https://img.shields.io/nuget/v/DbUp.svg)](https://www.nuget.org/packages/dbup) | [![NuGet](https://img.shields.io/nuget/vpre/DbUp.svg)](https://www.nuget.org/packages/dbup) |
| DbUp-SqlServer   | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlserver.svg)](https://www.nuget.org/packages/dbup-sqlserver) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlserver.svg)](https://www.nuget.org/packages/dbup-sqlserver) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlserver.svg)](https://www.nuget.org/packages/dbup-sqlserver) |
| DbUp-MySql       | [![NuGet](https://img.shields.io/nuget/dt/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) [![NuGet](https://img.shields.io/nuget/v/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-mysql.svg)](https://www.nuget.org/packages/dbup-mysql) |
| DbUp-SQLite      | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlite.svg)](https://www.nuget.org/packages/dbup-sqlite) |
| DbUp-SQLite-Mono | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) [![NuGet](https://img.shields.io/nuget/v/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlite-mono.svg)](https://www.nuget.org/packages/dbup-sqlite-mono) |
| DbUp-SqlCe       | [![NuGet](https://img.shields.io/nuget/dt/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce)  [![NuGet](https://img.shields.io/nuget/v/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-sqlce.svg)](https://www.nuget.org/packages/dbup-sqlce) |
| DbUp-PostgreSQL  | [![NuGet](https://img.shields.io/nuget/dt/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) [![NuGet](https://img.shields.io/nuget/v/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-postgresql.svg)](https://www.nuget.org/packages/dbup-postgresql) |
| DbUp-Redshift  | [![NuGet](https://img.shields.io/nuget/dt/dbup-redshift.svg)](https://www.nuget.org/packages/dbup-redshift) [![NuGet](https://img.shields.io/nuget/v/dbup-redshift.svg)](https://www.nuget.org/packages/dbup-redshift) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-redshift.svg)](https://www.nuget.org/packages/dbup-redshift) |
| DbUp-Firebird  | [![NuGet](https://img.shields.io/nuget/dt/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) [![NuGet](https://img.shields.io/nuget/v/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird) |
| DbUp-Oracle  | [![NuGet](https://img.shields.io/nuget/dt/dbup-oracle.svg)](https://www.nuget.org/packages/dbup-oracle) [![NuGet](https://img.shields.io/nuget/v/dbup-oracle.svg)](https://www.nuget.org/packages/dbup-oracle) | [![NuGet](https://img.shields.io/nuget/vpre/dbup-oracle.svg)](https://www.nuget.org/packages/dbup-oracle) |

## Getting Help
To learn more about DbUp check out the [documentation](https://dbup.readthedocs.io/en/latest/)

# Development
## Getting Started
Install:
- Sql CE 4.0 SP1 https://www.microsoft.com/en-us/download/details.aspx?id=30709
