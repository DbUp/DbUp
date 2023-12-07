using System.Collections.Generic;
using System.Linq;
using DbUp.Postgresql;
using Xunit;

namespace DbUp.Tests.Support.Postgresql;

public class PostgresqlQueryParserTests
{
    [Theory]
    [InlineData("SELECT 1\n;\nSELECT 2", 2, "SELECT 1", "SELECT 2")]
    [InlineData(";;SELECT 1", 1, "SELECT 1")]
    [InlineData("SELECT 1;", 1, "SELECT 1")]
    [InlineData("", 0)]
    [InlineData("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1; SELECT 1)",
        1,
        "CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1; SELECT 1)")]
    [InlineData("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1); SELECT 2",
        2,
        "CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1)", "SELECT 2")]
    [InlineData("SELECT 1 /* block comment; */", 1, "SELECT 1 /* block comment; */")]
    [InlineData(
        """
        SELECT 1;
        -- Line comment; with semicolon
        SELECT 2;
        """, 2,
        "SELECT 1",
        """
        -- Line comment; with semicolon
        SELECT 2
        """)]
    [InlineData("SELECT 'string with; semicolon'", 1, "SELECT 'string with; semicolon'")]
    [InlineData("SELECT 'string with'' quote and; semicolon'", 1, "SELECT 'string with'' quote and; semicolon'")]
    [InlineData("""
                CREATE FUNCTION TXT()
                LANGUAGE PLPGSQL AS
                $BODY$
                BEGIN
                    SELECT 'string with'' quote and; semicolon';
                END
                $BODY$
                """, 1)]
    [InlineData("SELECT 1 as \"QUOTED;IDENT\"", 1)]
    [InlineData("SELECT E'\\041'; SELECT '1'", 2, "SELECT E'\\041'", "SELECT '1'")]
    [InlineData("""
                SELECT 'some'
                'text';
                SELECT '1'
                """, 2)]
    public void split_into_statements(string sql, int statementCount, params string[] expected)
    {
        var results = ParseCommand(sql);
        Assert.Equal(statementCount, results.Count);
        if (expected.Length > 0)
            Assert.Equal(expected, results);
    }

    [Fact]
    public void split_into_statements_non_sql_standard()
    {
        const string sql = "SELECT 'string with\\' quote and; semicolon'";
        var results = ParseCommand(sql, false);
        Assert.Single(results);
        Assert.Equal(sql, results[0]);
    }

    private List<string> ParseCommand(string sql)
        => ParseCommand(sql, true);

    private static List<string> ParseCommand(string sql, bool standardConformingStrings)
    {
        var manager = new PostgresqlConnectionManager("") { StandardConformingStrings = standardConformingStrings };
        var commands = manager.SplitScriptIntoCommands(sql);
        return commands.ToList();
    }
}
