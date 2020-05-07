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
Reads upgrade scripts from a path on the file system.

### Usage

Read all files with a `.sql` extension from the location specified by `path`.
```
builder.WithScriptsFromFileSystem(path)
```

Locate all files with a `.sql` extension from the location specified by `path` then filter them using the lambda function.
```
builder.WithScriptsFromFileSystem(path, sqlFilePath => sqlFilePath.Contains("good"))
```

Read all files with a `.sql` extension from the location specified by `path` using the specified encoding.
```
builder.WithScriptsFromFileSystem(path, Encoding.UTF8)
```

Locate all files with a `.sql` extension from the location specified by `path`, then filter them using the lambda function, and read them using the specified encoding.
```
builder.WithScriptsFromFileSystem(path, sqlFilePath => sqlFilePath.Contains("good"), Encoding.UTF8)
```

Fully customise the options for the `FileSystemScriptProvider`.
```
var options = new FileSystemScriptOptions
{
  // true = scan into subdirectories, false = top directory only
  IncludeSubDirectories = true,
  
  // Patterns to search the file system for. Set to "*.sql" by default.
  Extensions = new [] { "*.sql" },
  
  // Type of text encoding to use when reading the files. Defaults to "Encoding.UTF8".
  Encoding = Encoding.UTF8,

  // Pass each file path located to this function and filter based on the result
  Filter = path => path.Contains("value")
}

builder.WithScriptsFromFileSystem(path, options);
```

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
