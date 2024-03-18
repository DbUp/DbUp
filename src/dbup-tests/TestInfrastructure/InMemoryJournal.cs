using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Output;

namespace DbUp.Tests.TestInfrastructure;

public class InMemoryJournal : IJournal
{
    readonly IUpgradeLog log;
    readonly List<string> executedScripts = new();

    public InMemoryJournal(IUpgradeLog log)
    {
        this.log = log;
    }

    public string[] GetExecutedScripts() => executedScripts.ToArray();

    public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory) => executedScripts.Add(script.Name);

    public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
    {
        log.LogInformation("Ensuring tables exists and is latest version");
    }

    public void AddScriptsAsPreviouslyExecuted(IReadOnlyList<SqlScript> scripts)
        => executedScripts.AddRange(scripts.Select(s => s.Name));
}
