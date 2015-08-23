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

        [Test]
        public void dbup_firebird_has_no_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(FirebirdExtensions).Assembly.Location);
        }

        [Test]
        public void dbup_mysql_has_no_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(MySqlExtensions).Assembly.Location);
        }

        [Test]
        public void dbup_postgres_has_no_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(PostgresqlExtensions).Assembly.Location);
        }

        [Test]
        public void dbup_sqlce_has_no_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(SqlCeExtensions).Assembly.Location);
        }

        [Test]
        public void dbup_sqlite_has_no_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(SQLiteExtensions).Assembly.Location);
        }
    }
}