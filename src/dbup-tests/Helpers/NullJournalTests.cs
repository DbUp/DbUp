using System.Data;
using DbUp.Engine;
using DbUp.Helpers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Helpers;

public class NullJournalTests
{
    [Fact]
    public void shouldnt_journal_anything()
    {
        var command = Substitute.For<IDbCommand>();

        var journal = new NullJournal();

        journal.StoreExecutedScript(new SqlScript("testscript", "SELECT * FROM BLAH"), () => command);
        journal.GetExecutedScripts().ShouldBeEmpty();
    }
}
