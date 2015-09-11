DbUp can get it's scripts which need to be executed from anywhere. Out of the box it has support for:

## EmbeddedScriptProvider
Finds scripts embedded in a single Assembly
### Usage
`builder.WithScriptsEmbeddedInAssembly(Assembly, [optional filter])`
### Example
`builder.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (string s) => s.StartsWith("Script"))`

## EmbeddedScriptsProvider
Finds scripts embedded in one or more assemblies
### Usage
`builder.WithScriptsEmbeddedInAssemblies(Assembly[], [optional filter])`
### Example

```
builder.WithScriptsEmbeddedInAssemblies(new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(Something).Assembly
},
(string s) => s.StartsWith("Script"))
```

## FileSystemScriptProvider
Finds scripts in a specified directory
### Usage
`builder.WithScriptsFromFileSystem(path)`

## StaticScriptProvider
Allows you to easily programatically supply scripts from code

### Usage
``` csharp
// Single script
builder.WithScript("name.sql", "content");
// Many scripts
builder.WithScripts(new[]
{
  new SqlScript("script1.sql", "content"),
  new SqlScript("script2.sql", "content2")
});
// Custom script provider
builder.WithScripts(new MyCustomScriptProvider());
```

## EmbeddedScriptAndCodeProvider
An enhanced script provider implementation which retrieves upgrade scripts or IScript code upgrade scripts embedded in an assembly.

**Warning:** DbUp does not protect against Sql Injection attacks, code scripts allow you to generate an upgrade script dynamically based on data in your database if you need to. If this data is put into the resulting script it could well contain a sql injection attack.

###Usage
`builder.WithScriptsAndCodeEmbeddedInAssembly(Assembly)`
