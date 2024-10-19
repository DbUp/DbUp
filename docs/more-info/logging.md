DbUp has a simple logging abstraction in place using the `IUpgradeLog` interface. Out of the box there are the following loggers:

* `ConsoleUpgradeLog` - Logs to `Console.Write*`.
    - Use `builder.LogToConsole()` to register.
* `TraceUpgradeLog` - Logs to `Trace.Write*`.
    - Use `builder.LogToTrace()` to register.
* `SqlContextUpgradeLog` - Logs to `SqlContext` (available when using the `dbup-sqlserver` package on .NET Framework).
    - Use `builder.LogToSqlContext()` to register.
* Use `builder.LogTo(new MyCustomLogger())` to provide your own logger based on `IUpgradeLog`.
* Use `builder.LogTo(ILogger)` or `builder.LogTo(ILoggerFactory)` to provide a [Microsoft.Extensions.Logging compliant logging provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers#third-party-logging-providers).

These calls use `builder.Configure((UpgradeConfiguration c) => c.AddLog(log))` under the covers.

If no logger is specified, no logging is enabled.

The first call to `upgradeConfigureation.AddLog(log)` will replace the default logger. Subsequent calls to `upgradeConfigureation.AddLog(log)` will combine the loggers and result in logs going to all specified loggers. To clear previously configured loggers call `builder.ResetConfiguredLoggers()`.

By default, the output of the scripts run by DbUp do not show up in the logs. To also display the script output (e.g.: text displayed by `PRINT` statements in Sql Server), use `builder.LogScriptOutput()`:

    Builder
      .LogToConsole()
      .LogScriptOutput()

Complete example:

    DeployChanges.To
      .SqlDatabase(connectionString)
      .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
      .LogToConsole()
      .LogScriptOutput()
      .Build()

Third-party logging provider example using Serilog:

    // Create the logging provider instance.
    var loggerFactory = new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger());

    DeployChanges.To
      .SqlDatabase(connectionString)
      .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
      .LogTo(loggerFactory) // Register the logging provider.
      .LogScriptOutput()
      .Build()
