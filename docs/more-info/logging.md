DbUp has a simple logging abstraction in place using the `IUpgradeLog` interface. Out of the box there are the following loggers:

* `ConsoleUpgradeLog` - Logs to `Console.Write*`
    - Use `builder.LogToConsole()` to register
* `SqlContextUpgradeLog` - Logs to `SqlContext`
    - Use `builder.LogToSqlContext()` to register
* `TraceUpgradeLog` - Logs to `Trace.Write*`
    - Use `builder.LogToTrace()` to register
* Use `builder.LogTo(new MyCustomLogger())` to provide your own logger

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


