using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;
using DbUp.Support;

/// <summary>
/// Configuration extensions for the standard stuff.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class StandardExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Logs to a custom logger.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="log">The logger.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogTo(this UpgradeEngineBuilder builder, IUpgradeLog log)
    {
        builder.Configure(c => c.AddLog(log));
        return builder;
    }

    /// <summary>
    /// Logs to the console using pretty colors.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToConsole(this UpgradeEngineBuilder builder)
    {
        return LogTo(builder, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Discards all log messages
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToNowhere(this UpgradeEngineBuilder builder)
    {
        return LogTo(builder, new NoOpUpgradeLog());
    }

#if SUPPORTS_LIBLOG
    /// <summary>
    /// Logs to a automatically detected globally configured logger supported by LibLog.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToAutodetectedLog(this UpgradeEngineBuilder builder)
    {
        return LogTo(builder, new AutodetectUpgradeLog());
    }
#endif

    /// <summary>
    /// Logs to the console using pretty colors.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogScriptOutput(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.ConnectionManager.IsScriptOutputLogged = true);
        return builder;
    }

    /// <summary>
    /// Logs to System.Diagnostics.Trace.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToTrace(this UpgradeEngineBuilder builder)
    {
        return LogTo(builder, new TraceUpgradeLog());
    }

    /// <summary>
    /// Resets any loggers configured with 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static UpgradeEngineBuilder ResetConfiguredLoggers(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.Log = null);
        return builder;
    }

    /// <summary>
    /// Uses a custom journal for recording which scripts were executed. A journal provided via this method
    /// does not participate in transactions
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="journal">The custom journal.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder JournalTo(this UpgradeEngineBuilder builder, IJournal journal)
    {
        builder.Configure(c => c.Journal = journal);
        return builder;
    }

    /// <summary>
    /// Uses a custom journal for recording which scripts were executed. This journal can participate in transactions
    /// if the provided IConnectionManager is used
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="createJournal">A function that takes a IConnectionManager factory and IUpgradeLog factory and returns the Journal to use</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder JournalTo(this UpgradeEngineBuilder builder, Func<Func<IConnectionManager>, Func<IUpgradeLog>, IJournal> createJournal)
    {
        builder.Configure(c => c.Journal = createJournal(() => c.ConnectionManager, () => c.Log));
        return builder;
    }

    /// <summary>
    /// Adds a custom script provider to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scriptProvider">The script provider.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, IScriptProvider scriptProvider)
    {
        builder.Configure(c => c.ScriptProviders.Add(scriptProvider));
        return builder;
    }

    /// <summary>
    /// Adds a static set of scripts to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scripts">The scripts.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, IEnumerable<SqlScript> scripts)
    {
        return WithScripts(builder, new StaticScriptProvider(scripts));
    }

    /// <summary>
    /// Adds a static set of scripts to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scripts">The scripts.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, params SqlScript[] scripts)
    {
        return WithScripts(builder, (IEnumerable<SqlScript>)scripts);
    }

    /// <summary>
    /// Adds a single static script to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="script">The script.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, SqlScript script)
    {
        return WithScripts(builder, script);
    }

    /// <summary>
    /// Adds a single static script to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name of the script. This should never change once executed.</param>
    /// <param name="contents">The script body.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, string name, string contents)
    {
        var script = new SqlScript(name, contents);
        return WithScripts(builder, script);
    }

    /// <summary>
    /// Adds a single static script to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name of the script. This should never change once executed.</param>
    /// <param name="contents">The script body.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, string name, string contents, SqlScriptOptions sqlScriptOptions)
    {
        var script = new SqlScript(name, contents, sqlScriptOptions);
        return WithScripts(builder, script);
    }

    /// <summary>
    /// Adds a single IScript instance to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name of the script</param>
    /// <param name="script">The script instance</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, string name, IScript script)
        => WithScripts(builder, new ScriptInstanceProvider(_ => name, script));

    /// <summary>
    /// Adds a single IScript instance to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name of the script</param>
    /// <param name="script">The script instance</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScript(this UpgradeEngineBuilder builder, string name, IScript script, SqlScriptOptions sqlScriptOptions)
        => WithScripts(builder, new ScriptInstanceProvider(_ => name, sqlScriptOptions, script));

    /// <summary>
    /// Adds IScript instances to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scripts">The script instances.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, params IScript[] scripts)
        => WithScripts(builder, new ScriptInstanceProvider(scripts));

    /// <summary>
    /// Adds IScript instances to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="namer">A function that returns the name of the script</param>
    /// <param name="scripts">The script instances.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, Func<IScript, string> namer, params IScript[] scripts)
        => WithScripts(builder, new ScriptInstanceProvider(namer, scripts));

    /// <summary>
    /// Adds IScript instances to the upgrader.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="namer">A function that returns the name of the script</param>
    /// <param name="scripts">The script instances.</param>
    /// <param name="sqlScriptOptions">The script sql script options.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScripts(this UpgradeEngineBuilder builder, Func<IScript, string> namer, SqlScriptOptions sqlScriptOptions, params IScript[] scripts)
        => WithScripts(builder, new ScriptInstanceProvider(namer, sqlScriptOptions, scripts));

    /// <summary>
    /// Adds all scripts from a folder on the file system.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>    
    /// <param name="sqlScriptOptions">The sql script options</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions(), sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with a custom filter.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="filter">The filter. Use the static <see cref="Filters"/> class to get some pre-defined filters.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, Func<string, bool> filter)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions() { Filter = filter }));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with a custom filter and custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="filter">The filter. Use the static <see cref="Filters"/> class to get some pre-defined filters.</param>    
    /// <param name="sqlScriptOptions">The sql script options</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, Func<string, bool> filter, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions() { Filter = filter }, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, Encoding encoding)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions() { Encoding = encoding }));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="sqlScriptOptions">The sql script options</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, Encoding encoding, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions() { Encoding = encoding }, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with a custom filter and custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="filter">The filter. Use the static <see cref="Filters"/> class to get some pre-defined filters.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, Func<string, bool> filter, Encoding encoding)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions() { Filter = filter, Encoding = encoding }));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with a custom filter and custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="filter">The filter. Use the static <see cref="Filters"/> class to get some pre-defined filters.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="sqlScriptOptions">The sql script options</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, Func<string, bool> filter, Encoding encoding, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, new FileSystemScriptOptions() { Filter = filter, Encoding = encoding }, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts from a folder on the file system, with custom options (Encoding, filter, etc.).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="options">Options for the file System Provider</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsFromFileSystem(this UpgradeEngineBuilder builder, string path, FileSystemScriptOptions options)
    {
        return WithScripts(builder, new FileSystemScriptProvider(path, options));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly and assigns them the script options.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), DbUpDefaults.DefaultEncoding, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with custom encoding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Encoding encoding)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), encoding));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with custom encoding and script type.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Encoding encoding, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), encoding, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with custom encoding and with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter, Encoding encoding)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, filter, encoding));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with custom encoding and with a custom filter (you'll need to exclude non- .SQL files yourself) and where you specify the script type.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter, Encoding encoding, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, filter, encoding, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, filter));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, with a custom filter (you'll need to exclude non- .SQL files yourself) and where you specify the script type.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptProvider(assembly, filter, DbUpDefaults.DefaultEncoding, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), s => true));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The script filter. Don't forget to ignore any non- .SQL files.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, filter));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The script filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="codeScriptFilter">The embedded script filter.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter, Func<string, bool> codeScriptFilter)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, filter, codeScriptFilter));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The script filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="codeScriptFilter">The embedded script filter.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter, Func<string, bool> codeScriptFilter, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, filter, codeScriptFilter, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assembly, or classes which inherit from IScript, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filter">The script filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsAndCodeEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, Func<string, bool> filter, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptAndCodeProvider(assembly, filter, sqlScriptOptions));
    }

    /// <summary>
    /// Sets the comparer used to sort scripts and match script names against the log of already run scripts.
    /// The default comparer is StringComparer.Ordinal.
    /// By implementing your own comparer you can make the matching and ordering case insensitive,
    /// change how numbers are handled or support the renaming of scripts
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="comparer">The sorter.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptNameComparer(this UpgradeEngineBuilder builder, IComparer<string> comparer)
    {
        builder.Configure(b => b.ScriptNameComparer = new ScriptNameComparer(comparer));
        return builder;
    }

    /// <summary>
    /// Sets the filter that filters the sorted lists of script prior to execution. This allows
    /// scripts to be excluded based on which scripts have already been run.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithFilter(this UpgradeEngineBuilder builder, IScriptFilter filter)
    {
        builder.Configure(b => b.ScriptFilter = filter);
        return builder;
    }

    /// <summary>
    /// Adds a preprocessor that can replace portions of a script.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="preprocessor">The preprocessor.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithPreprocessor(this UpgradeEngineBuilder builder, IScriptPreprocessor preprocessor)
    {
        builder.Configure(c => c.ScriptPreprocessors.Add(preprocessor));
        return builder;
    }

    /// <summary>
    /// Adds a set of variables that will be replaced before scripts are executed.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="variables">The variables.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithVariables(this UpgradeEngineBuilder builder, IDictionary<string, string> variables)
    {
        builder.Configure(c => c.AddVariables(variables));
        return builder;
    }

    /// <summary>
    /// Adds a single variable that will be replaced before scripts are executed.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="value">The value to be substituted.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithVariable(this UpgradeEngineBuilder builder, string variableName, string value)
    {
        return WithVariables(builder, new Dictionary<string, string> { { variableName, value } });
    }

    /// <summary>
    /// Sets a configuration flag which will cause the engine to skip variable expansion.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithVariablesDisabled(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.VariablesEnabled = false);
        return builder;
    }

    /// <summary>
    /// Sets a configuration flag which will cause the engine to perform variable expansion.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithVariablesEnabled(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.VariablesEnabled = true);
        return builder;
    }

    /// <summary>
    /// Allows you to set the execution timeout for scripts.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="timeout">A <c>TimeSpan</c> value containing the timeout value or <c>null</c>.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">The timeout value is less than zero or greater than 2,147,483,647 seconds.</exception>
    /// <remarks>Setting the timeout parameter to <c>null</c> will use the default timeout of the underlying provider.</remarks>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithExecutionTimeout(this UpgradeEngineBuilder builder, TimeSpan? timeout)
    {
        if (timeout == null)
        {
            builder.Configure(c => c.ScriptExecutor.ExecutionTimeoutSeconds = null);
            return builder;
        }

        var totalSeconds = timeout.Value.TotalSeconds;

        if ((0 > totalSeconds) || (totalSeconds > int.MaxValue)) throw new ArgumentOutOfRangeException("timeout", timeout, string.Format("The timeout value must be a value between 0 and {0} seconds", int.MaxValue));

        builder.Configure(c => c.ScriptExecutor.ExecutionTimeoutSeconds = Convert.ToInt32(totalSeconds));
        return builder;
    }

    /// <summary>
    /// Run creates a new connection for each script, without a transaction
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithoutTransaction(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.ConnectionManager.TransactionMode = TransactionMode.NoTransaction);

        return builder;
    }

    /// <summary>
    /// Run DbUp in a single transaction
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithTransaction(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.ConnectionManager.TransactionMode = TransactionMode.SingleTransaction);

        return builder;
    }

    /// <summary>
    /// Run DbUp in a single transaction but rollback at the end
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithTransactionAlwaysRollback(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.ConnectionManager.TransactionMode = TransactionMode.SingleTransactionAlwaysRollback);

        return builder;
    }

    /// <summary>
    /// Run each script in it's own transaction
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static UpgradeEngineBuilder WithTransactionPerScript(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.ConnectionManager.TransactionMode = TransactionMode.TransactionPerScript);

        return builder;
    }

    /// <summary>
    /// Adds all scripts ending in '.sql' found as embedded resources in the given assemblies, using the default <see cref="Encoding" />.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this UpgradeEngineBuilder builder, Assembly[] assemblies)
    {
        return WithScriptsEmbeddedInAssemblies(builder, assemblies, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Adds all scripts matching the specified filter found as embedded resources in the given assemblies, using the UTF8 <see cref="Encoding" />.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this UpgradeEngineBuilder builder, Assembly[] assemblies, Func<string, bool> filter)
    {
        return WithScriptsEmbeddedInAssemblies(builder, assemblies, filter, DbUpDefaults.DefaultEncoding);
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assemblies, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>    
    /// <param name="sqlScriptOptions">The sql script options.</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this UpgradeEngineBuilder builder, Assembly[] assemblies, Func<string, bool> filter, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptsProvider(assemblies, filter, DbUpDefaults.DefaultEncoding, sqlScriptOptions));
    }

    /// <summary>
    /// Adds all scripts ending in '.sql' found as embedded resources in the given assemblies, using the specified <see cref="Encoding" />.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this UpgradeEngineBuilder builder, Assembly[] assemblies, Encoding encoding)
    {
        return WithScriptsEmbeddedInAssemblies(builder, assemblies, s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), encoding);
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assemblies, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this UpgradeEngineBuilder builder, Assembly[] assemblies, Func<string, bool> filter, Encoding encoding)
    {
        return WithScripts(builder, new EmbeddedScriptsProvider(assemblies, filter, encoding));
    }

    /// <summary>
    /// Adds all scripts found as embedded resources in the given assemblies, with a custom filter (you'll need to exclude non- .SQL files yourself).
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="filter">The filter. Don't forget to ignore any non- .SQL files.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>    
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder WithScriptsEmbeddedInAssemblies(this UpgradeEngineBuilder builder, Assembly[] assemblies, Func<string, bool> filter, Encoding encoding, SqlScriptOptions sqlScriptOptions)
    {
        return WithScripts(builder, new EmbeddedScriptsProvider(assemblies, filter, encoding, sqlScriptOptions));
    }
}
