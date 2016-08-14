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

**Note:** there is no way to escape variables, if this causes you issues, create a GitHub issue or submit a pull request to allow escaping!
