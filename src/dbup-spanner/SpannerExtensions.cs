using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.Spanner;

/// <summary>
/// Configuration extension methods for Spanner.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the global:: namespace so that it is always available
// ReSharper disable CheckNamespace
public static class SpannerExtensions
{
    public static UpgradeEngineBuilder SpannerDatabase(this SupportedDatabases supported, string connectionString, string schema = null)
    {
        return SpannerDatabase(new SpannerConnectionManager(connectionString), schema);
    }

    public static UpgradeEngineBuilder SpannerDatabase(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SpannerScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new SpannerTableJournal(() => c.ConnectionManager, () => c.Log, schema, "schemaversions"));
        builder.WithPreprocessor(new SpannerPreprocessor());
        return builder;
    }
}
