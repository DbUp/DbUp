Script Pre-Processors are a really handy extensibility hook into DbUp, it allows you to modify a script before it is executed. Some examples of how this can/is used:

* Variable Substitution
* Replacing incompatible types
  - For SqlCe/SQLite we automatically replace `nvarchar(max)` with `ntext`
* Stripping the $schema$. variable out of databases which do not support schemas

## Writing your own
To create your own pre-processor just implement the `IScriptPreprocessor` interface then register is with your DbUp builder:

``` csharp
DeployChanges
  .To
  .SqlDatabase(..)
  .WithPreProcessor(new MyPreprocessor())
```
