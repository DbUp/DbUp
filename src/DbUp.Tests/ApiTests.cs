using DbUp.Builder;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests
{
    [TestFixture]
    public class ApiTests
    {
        [Test]
        public void dbup_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(SupportedDatabases).Assembly)
                .ShouldMatchApproved();
        }

        [Test]
        public void dbup_firebird_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(FirebirdExtensions).Assembly)
                .ShouldMatchApproved();
        }

        [Test]
        public void dbup_mysql_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(MySqlExtensions).Assembly)
                .ShouldMatchApproved();
        }

        [Test]
        public void dbup_postgres_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(PostgresqlExtensions).Assembly)
                .ShouldMatchApproved();
        }

        [Test]
        public void dbup_sqlce_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(SqlCeExtensions).Assembly)
                .ShouldMatchApproved();
        }

        [Test]
        public void dbup_sqlite_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(SQLiteExtensions).Assembly)
                .ShouldMatchApproved();
        }
    }
}