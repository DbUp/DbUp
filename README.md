
## Modified version of DbUp

Original https://github.com/DbUp/DbUp

Changes added support for Database Project, Migration scripts, Pre & Post deployment. Extended migration table.

Added `PreProcessors`
`ErrorHandlingPreProcessor` -- Adds some error handling to the script that will prevent further execution of migration scripts

`FunctionCreateOrAlterPreProcessor` -- Ensures all database project Functions are CREATE OR ALTER FUNCTION instead of just CREATE FUNCTION

`RoleCreateIfNotExistsPreProcessor` -- Ensures all CREATE ROLE statements are prefixes with an if exists

`SqlCmdVariablePreProcessor` -- Replaces any SQLCmdVariables $(YourVar)

`StoredProcedureCreateOrAlterPreProcessor` -- Ensures all database project Store Procs are CREATE OR ALTER PROC instead of just CREATE PROC

`ViewCreateOrAlterPreProcessor`-- Ensures all database project Views are CREATE OR ALTER VIEW instead of just CREATE VIEW


Extends the migration history table with Hash, Contents, Group & DeploymentId

Hash - SHA256 of file content
Contents - The script that was executed (after pre-processors)
DeploymentId - A unique Id for the whole deploymeny
Group - The group in which the script came from. Pre-Deployment, Post-Deployment, Migrations, Programmable Objects

Added additional `ScriptType`, `ScriptType.RunHash`, this will only execute the script if the content has changed or has not been run
