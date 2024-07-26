# 4.0.0

 - If `builder.LogTo..()` is called multiple times, the logs are combined instead of replacing the previous logger
 - Default Encoding is now UTF rather than Encoding.Default
 - AdHocSqlRunner changed to use `Expression<Func<>>` rather than `Func<>`
 - EmbeddedScriptAndCodeProvider (used by the extension method WithScriptsAndCodeEmbeddedInAssembly) now also applies the given filter to the code based scripts
 - `IJournal` now has a new method `EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)`
 - The `protected void EnsureTableIsLatestVersion()` method in `TableJournal` has changed to `public virtual void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)`

# 6.0.0

- LibLog has been replaced by the standard .NET ILogger abstraction
    - The Autodetect logger feature provided by LibLog is removed
    - `MultipleUpgradeLog` has been replaced with `AggregateLog`
    - `IUpgradeLogger` still exists but method names have changed from `Write*` to `Log*` to match Microsoft's `ILogger` interface
- Scripts with `ScriptType.RunAlways` [will no longer be journaled](https://github.com/DbUp/DbUp/issues/789)
 - Only `netstandard2.0` compatible frameworks are supported
    - .NET 5.0 and later
    - .NET Core 2.0 and later
    - .NET Framework 4.6.1
- All dependencies have been updated to the latest available release version
- SqlServer Provider: Replaced `System.Data.SqlClient` with `Microsoft.Data.SqlClient`
- MySql Provider: [Use `MySqlConnector` instead of `System.Data.SqlClient`](https://github.com/DbUp/dbup-mysql/pull/9)