using System;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Support;

namespace DbUp.Tests.TestInfrastructure;

public class TestScriptExecutor : ScriptExecutor
{
    public TestScriptExecutor(UpgradeConfiguration c, string schema) : base(
        () => c.ConnectionManager,
        new TestSqlObjectParser(),
        () => c.Log,
        schema,
        () => c.VariablesEnabled,
        c.ScriptPreprocessors,
        () => c.Journal
    )
    {
    }

    protected override string GetVerifySchemaSql(string schema)
        => string.Format(@"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA [{0}]')", Schema);

    protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action executeCallback) => executeCallback();

    class TestSqlObjectParser : SqlObjectParser
    {
        public TestSqlObjectParser() : base("[", "]")
        {
        }
    }
}
