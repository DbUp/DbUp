using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.ScriptProviders;
using DbUp.Tests.Common;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;

namespace DbUp.Tests.Helpers;

public class UpgradeEngineHtmlReportTests : IDisposable
{
    private readonly List<string> createdFiles = new();

    [Fact]
    public void Should_generate_html_file_with_basic_structure()
    {
        var testFilePath = Path.Combine(Path.GetTempPath(), $"test-report-{Guid.NewGuid()}.html");
        createdFiles.Add(testFilePath);

        var dbConnection = Substitute.For<IDbConnection>();
        var connectionManager = new TestConnectionManager(dbConnection);

        var script1 = new SqlScript("Script001.sql", "CREATE TABLE Test1 (Id INT)");
        var script2 = new SqlScript("Script002.sql", "CREATE TABLE Test2 (Name VARCHAR(100))");

        var builder = new UpgradeEngineBuilder()
            .WithScript(script1)
            .WithScript(script2)
            .JournalTo(new NullJournal());
        builder.Configure(c => c.ScriptExecutor = new TestScriptExecutor(c, "dbo"));
        builder.Configure(c => c.ConnectionManager = connectionManager);

        var upgrader = builder.Build();

        upgrader.GenerateUpgradeHtmlReport(testFilePath);

        File.Exists(testFilePath).ShouldBeTrue();

        var htmlContent = File.ReadAllText(testFilePath);

        htmlContent.ShouldContain("<!DOCTYPE html>");
        htmlContent.ShouldContain("<html>");
        htmlContent.ShouldContain("</html>");
        htmlContent.ShouldContain("<head>");
        htmlContent.ShouldContain("</head>");
        htmlContent.ShouldContain("<body>");
        htmlContent.ShouldContain("</body>");
        htmlContent.ShouldContain("DBUp Delta Report");
        htmlContent.ShouldContain("Script001.sql");
        htmlContent.ShouldContain("Script002.sql");
        htmlContent.ShouldContain("CREATE TABLE Test1 (Id INT)");
        htmlContent.ShouldContain("CREATE TABLE Test2 (Name VARCHAR(100))");
        htmlContent.ShouldContain("bootstrap");
        htmlContent.ShouldContain("jquery");
    }

    [Fact]
    public void Should_generate_html_file_with_embedded_sql_and_code_scripts()
    {
        var testFilePath = Path.Combine(Path.GetTempPath(), $"test-report-with-code-{Guid.NewGuid()}.html");
        createdFiles.Add(testFilePath);

        var dbConnection = Substitute.For<IDbConnection>();
        var connectionManager = new TestConnectionManager(dbConnection);

        var assembly = Assembly.GetExecutingAssembly();
        var scriptProvider = new EmbeddedScriptAndCodeProvider(assembly, s => s.Contains("Test4") || s.Contains("Test1"));

        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ScriptProviders.Add(scriptProvider));
        builder.Configure(c => c.Journal = new NullJournal());
        builder.Configure(c => c.ScriptExecutor = new TestScriptExecutor(c, "dbo"));
        builder.Configure(c => c.ConnectionManager = connectionManager);

        var upgrader = builder.Build();

        upgrader.GenerateUpgradeHtmlReport(testFilePath);

        File.Exists(testFilePath).ShouldBeTrue();

        var htmlContent = File.ReadAllText(testFilePath);

        htmlContent.ShouldContain("<!DOCTYPE html>");
        htmlContent.ShouldContain("DBUp Delta Report");
        htmlContent.ShouldContain("Test1");
        htmlContent.ShouldContain("Script20120723_1_Test4.cs");
        htmlContent.ShouldContain("test4");
    }

    public void Dispose()
    {
        foreach (var file in createdFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
