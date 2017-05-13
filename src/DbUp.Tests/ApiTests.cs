using ApiApprover;
using ApprovalTests.Reporters;
using DbUp.Builder;
using NUnit.Framework;

namespace DbUp.Tests
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class ApiTests
    {
        [Test]
        public void dbup_has_no_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(SupportedDatabases).Assembly.Location);
        }
    }
}