DbUp supports basic variable substitution, to enable you should register variables when configuring DbUp:

``` csharp
DeployChanges.To
  .SqlDatabase(..)
  .WithVariable("TestVariable", "Value")
```

Then in your database script:

```
print '$TestVariable$'
```

Will execute `print 'Value'`

or, you can use the same syntax used by SQLCMD [Scripting Variables](https://msdn.microsoft.com/en-us/library/ms188714.aspx)

```
print '$(TestVariable)'
```

This will also execute `print 'Value'`

**Note:** there is no way to escape variables, if this causes you issues, create a GitHub issue or submit a pull request to allow escaping!
