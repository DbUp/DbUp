DbUp has a simple logging abstraction in place using the `IUpgradeLog` interface. Out of the box there are the following loggers:

* `AutodetectUpgradeLog` - Uses [LibLog](https://github.com/damianh/LibLog) to automatically detect the configured logging framework. It automatically detects [Serilog](https://serilog.net/), [NLog](http://nlog-project.org/), [Log4Net](https://logging.apache.org/log4net/), [EntLib](https://msdn.microsoft.com/en-us/library/ff648951.aspx) and [Loupe](https://onloupe.com/). Available on .NET 4.5+ and .NET Core.
    - Use `builder.LogToAutodetectedLog()` to register
* `ConsoleUpgradeLog` - Logs to `Console.Write*`
    - Use `builder.LogToConsole()` to register
* `TraceUpgradeLog` - Logs to `Trace.Write*`
    - Use `builder.LogToTrace()` to register
* `NoOpUpgradeLog` - No logging
    - Use `builder.LogToNowhere()` to discard all logs
* `SqlContextUpgradeLog` - Logs to `SqlContext` (available when using the `dbup-sqlserver` package on .NET Framework)
    - Use `builder.LogToSqlContext()` to register
* Use `builder.LogTo(new MyCustomLogger())` to provide your own logger

These calls use `builder.Configure((UpgradeConfigureation c) => c.AddLog(log))` under the covers.

If no logger is specified, the `AutodetectUpgradeLog` is used for .NET 4.5+ and .NET Core. `TraceUpgradeLog` is used on earlier .NET frameworks.

The first call to `upgradeConfigureation.AddLog(log)` will replace the default logger. Subsequent calls to `upgradeConfigureation.AddLog(log)` will combine the loggers and result in logs going to all specified loggers. To clear previously configured loggers call `UpgradeConfigureation.Log = null` or `builder.ResetConfiguredLoggers()`.

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


