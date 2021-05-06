# DBOps

DBOps is a Powershell module based on DbUp that provides Continuous Integration/Continuous Deployment capabilities for SQL database deployments. It comes with an easy-to-use set of functions developed to enhance and simplify the DbUp experience for Powershell users. Along with multiple configuration and script deployment options, it also comes with an ability to organize scripts into ready-to-deploy packages, creating artifacts for your deployment pipeline. Such packages can now be deployed in a straightforward and fully repeatable manner.

RDBMS currently supported by DBOps:
* SQL Server
* Oracle
* PostgreSQL
* MySQL

## Features
The most notable features of the module:

* Reliably deploy your scripts in a consistent and repeatable manner
* Perform ad-hoc deployments with highly customizable deployment parameters
* Run ad-hoc queries to any supported RDBMS on both Windows and Linux
* Create ready-to-deploy versioned packages in a single command
* Brings along all features of CI/CD pipelining functionality: builds, artifact management, deployment
* Native DbUp transaction management and schema versioning
* Dynamically change your code based on custom variables using `#{customVarName}` tokens


## System requirements

* Powershell 5.0 or higher

## Installation

### From PSGallery
You can always install the module through Powershell Gallery:
```powershell
Install-Module dbops
```

### Using git
When using git, make sure to also install dependant modules (see below):
```powershell
git clone https://github.com/sqlcollaborative/dbops.git dbops
Import-Module .\dbops
```

### Module dependencies
- [PSFramework](https://github.com/PowershellFrameworkCollective/psframework)
- [ZipHelper](https://www.powershellgallery.com/packages/ziphelper) - only if you intend to run module tests


## Use cases

* Ad-hoc deployments of any scale without manual code execution
* Delivering new version of the database schema in a consistent manner to multiple environments
* Build/Test/Deploy scenarios inside the Continuous Integration/Continuous Delivery pipeline
* Dynamic deployment based on modified files in the source folder
* Versioned package deployment (e.g. Octopus Deployment)
* Octopus Deploy database deployments

## Getting started
### SQL script deployments and running DB queries

```powershell
# Ad-hoc deployment of the scripts from a folder myscripts
Install-DBOSqlScript -ScriptPath C:\temp\myscripts -SqlInstance server1 -Database MyDB

# Execute a list of files as an Ad-hoc query
Get-ChildItem C:\temp\myscripts | Invoke-DBOQuery -SqlInstance server1 -Database MyDB
```
### Package management

Packages are designed to become an artifact in your CI/CD pipeline. They come as ready-to-deploy zip containers, that include database scripts, deployment configuration and the module itself.

<img src="https://sqlcollaborative.github.io/dbops/img/dbops-package.jpg" alt="dbops packages" width="800"/>

Each package consists of multiple builds and can be easily deployed to the target database, ensuring that each script in the build is deployed in proper order and only once.

DBOps packages are compatible with Octopus Deploy framework, having a `deploy.ps1` file inside that initiates the deployment.

```powershell
# Deployment using packaging system
New-DBOPackage Deploy.zip -ScriptPath C:\temp\myscripts | Install-DBOPackage -SqlInstance server1 -Database MyDB

# Create new deployment package with predefined configuration and deploy it replacing #{dbName} tokens with corresponding values
New-DBOPackage -Path MyPackage.zip -ScriptPath .\Scripts -Configuration @{ Database = '#{dbName}'; ConnectionTimeout = 5 }
Install-DBOPackage MyPackage.zip -Variables @{ dbName = 'myDB' }

# Adding builds to the package
Add-DBOBuild Deploy.zip -ScriptPath .\myscripts -Type Unique -Build 2.0
Get-ChildItem .\myscripts | Add-DBOBuild Deploy.zip -Type New,Modified -Build 3.0

# Install package using internal script Deploy.ps1; useful when module is not installed locally
Expand-Archive Deploy.zip '.\MyTempFolder'
.\MyTempFolder\Deploy.ps1 -SqlInstance server1 -Database MyDB
```
### Configurations and defaults

There are multiple configuration options available, including:
* Configuring default settings
* Specifying runtime parameters
* Using configuration files

```powershell
# Setting deployment options within the package to be able to deploy it without specifying options
Update-DBOConfig Deploy.zip -Configuration @{ DeploymentMethod = 'SingleTransaction'; SqlInstance = 'localhost'; Database = 'MyDb2' }
Install-DBOPackage Deploy.zip

# Generating config files and using it later as a deployment template
New-DBOConfig -Configuration @{ DeploymentMethod = 'SingleTransaction'; SqlInstance = 'devInstance'; Database = 'MyDB' } | Export-DBOConfig '.\dev.json'
Get-DBOConfig -Path '.\dev.json' -Configuration @{ SqlInstance = 'prodInstance' } | Export-DBOConfig '.\prod.json'
# Deploy package
Install-DBOPackage Deploy.zip -ConfigurationFile .\dev.json
# Deploy scripts
Install-DBOSqlScript .\MyScripts*.sql -ConfigurationFile .\prod.json

# Invoke package deployment using custom connection string
Install-DBOPackage -Path Deploy.zip -ConnectionString 'Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;'

# Invoke package deployment to an Oracle database OracleDB
Install-DBOPackage -Path Deploy.zip -Server OracleDB -ConnectionType Oracle

# Get a list of all the default settings
Get-DBODefaultSetting

# Change the default SchemaVersionTable setting to null, disabling the deployment journalling by default
Set-DBODefaultSetting -Name SchemaVersionTable -Value $null

# Reset SchemaVersionTable setting back to its default value
Reset-DBODefaultSetting -Name SchemaVersionTable
```
### CI/CD features

DBOps CI/CD flow assumes that each package version is built only once and deployed onto every single environment. The successfull builds should make their way as artifacts into the artifact storage, from which they would be pulled again to add new builds into the package during the next iteration.

<img src="https://sqlcollaborative.github.io/dbops/img/ci-cd-flow.jpg" alt="CI-CD flow" width="800"/>

CI/CD capabilities of the module enable user to integrate SQL scripts into a package file using a single command and to store packages in a versioned package repository.

```powershell
# Invoke CI/CD build of the package MyPackage.zip using scripts from the source folder .\Scripts
# Each execution of the command will only pick up new files from the ScriptPath folder
Invoke-DBOPackageCI -Path MyPackage.zip -ScriptPath .\Scripts -Version 1.0

# Store the package in a DBOps package repository in a folder \\data\repo
Publish-DBOPackageArtifact -Path myPackage.zip -Repository \\data\repo

# Retrieve the latest package version from the repository and install it
Get-DBOPackageArtifact -Path myPackage.zip -Repository \\data\repo | Install-DBOPackage -Server MyDBServer -Database MyDB
```

## More info
[DBOps Github](https://github.com/sqlcollaborative/dbops/)
