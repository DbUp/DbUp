using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using DbUp.Engine;

namespace DbUp.Tests.Common.RecordingDb;

public class RecordingDbCommand : IDbCommand
{
    readonly CaptureLogsLogger logger;
    readonly Dictionary<string, Func<object>> scalarResults;
    readonly Dictionary<string, Func<int>> nonQueryResults;

    public RecordingDbCommand(
        CaptureLogsLogger logger,
        Dictionary<string, Func<object>> scalarResults,
        Dictionary<string, Func<int>> nonQueryResults
    )
    {
        this.logger = logger;
        this.scalarResults = scalarResults;
        this.nonQueryResults = nonQueryResults;
        Parameters = new RecordingDataParameterCollection(logger);
    }

    public void Dispose()
    {
        logger.WriteDbOperation("Dispose command");
    }

    public void Prepare()
    {
        throw new NotImplementedException();
    }

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public IDbDataParameter CreateParameter()
    {
        logger.WriteDbOperation("Create parameter");
        return new RecordingDbDataParameter();
    }

    public int ExecuteNonQuery()
    {
        logger.WriteDbOperation($"Execute non query command: {CommandText}");

        if (CommandText == "error")
            ThrowError();

        if (nonQueryResults.ContainsKey(CommandText))
            return nonQueryResults[CommandText]();
        return 0;
    }

    void ThrowError()
    {
        throw new TestDbException();
    }

    public IDataReader ExecuteReader()
    {
        logger.WriteDbOperation($"Execute reader command: {CommandText}");

        if (CommandText == null)
            throw new InvalidOperationException("CommandText must be set before calling");

        if (CommandText == "error")
            ThrowError();

        return new EmptyReader();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        throw new NotImplementedException();
    }

    public object? ExecuteScalar()
    {
        logger.WriteDbOperation($"Execute scalar command: {CommandText}");

        if (CommandText == null)
            throw new InvalidOperationException("CommandText must be set before calling");

        if (CommandText == "error")
            ThrowError();

        // Are we checking if schemaversions exists
        if (CommandText.IndexOf("SchemaVersions", StringComparison.OrdinalIgnoreCase) != -1)
        {
            return 0;
        }

        if (scalarResults.ContainsKey(CommandText))
        {
            return scalarResults[CommandText]();
        }

        return null;
    }

    public IDbConnection? Connection { get; set; }

    public IDbTransaction? Transaction { get; set; }

    /// <summary>
    /// Set to 'error' to throw when executed
    /// </summary>
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public string CommandText { get; set; } = "";
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

    public int CommandTimeout { get; set; }

    public CommandType CommandType { get; set; }

    public IDataParameterCollection Parameters { get; }

    public UpdateRowSource UpdatedRowSource { get; set; }

    class TestDbException : DbException;
}
