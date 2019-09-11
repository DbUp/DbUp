DbUp supports basic variable substitution, to enable you should register variables when configuring DbUp:

``` csharp
DeployChanges.To
  .SqlDatabase(..)
  .WithVariable("TestVariable", "Value")
```

For more than one variable, use a Dictionary:
``` csharp
var myVariables = new Dictionary<string, string>
{
  {"Variable1", "Value1"},
  {"Variable2", "Value2"},
  {"Variable3", "Value3"},
  ...
};

DeployChanges.To
  .SqlDatabase(..)
  .WithVariables(myVariables)
```

Then in your database script, you need to enclose all variables in ```$``` signs, for example ```$variable$```:

```
-- $TestVariable$ $AnotherVariable$
print '$TestVariable$'
SELECT * FROM dbo.$TestVariable$
```

Will result in:
```
print 'Value'
SELECT * FROM dbo.Value
```

Variables can only contain letters, digits, `_` and `-`. If there are any other characters between the `$`s it is not treated as a variable. If a variable is found within a script, but not supplied an exception will be thrown unless it is within a comment.

**Note:** there is no way to escape variables, if this causes you issues, create a GitHub issue or submit a pull request to allow escaping!
