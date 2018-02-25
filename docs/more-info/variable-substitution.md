DbUp supports basic variable substitution, to enable you should register variables when configuring DbUp:

``` csharp
DeployChanges.To
  .SqlDatabase(..)
  .WithVariable("TestVariable", "Value")
```

Then in your database script:

```
-- $TestVariable$ $AnotherVariable$
print '$TestVariable$'
SELECT * FROM dbo.$TestVariable
```

Will execute:
```
-- Value $AnotherVariable$
print 'Value'
SELECT * FROM dbo.Value
```

Variables can only contain letters, digits, `_` and `-`. If there are any other characters between the `$`s it is not treated as a variable. If a variable is found within a script, but not supplied an exception will be thrown unless it is within a comment.

**Note:** there is no way to escape variables, if this causes you issues, create a GitHub issue or submit a pull request to allow escaping!
