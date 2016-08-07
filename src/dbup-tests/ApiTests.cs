#if !NETCORE
using DbUp.Builder;
using Shouldly;
using Xunit;

namespace DbUp.Tests
{
    public class ApiTests
    {
        [Fact]
        public void dbup_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(SupportedDatabases).Assembly)
                .ShouldMatchApproved(b => b
                    .WithFilenameGenerator((info, descriminator, type, extension) => $"DbUp.{type}.cs")
                    .SubFolder("ApprovalFiles"));
        }

        [Fact]
        public void dbup_firebird_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(FirebirdExtensions).Assembly)
                .ShouldMatchApproved(b => b
                    .WithFilenameGenerator((info, descriminator, type, extension) => $"DbUp.Firebird.{type}.cs")
                    .SubFolder("ApprovalFiles"));
        }

        [Fact]
        public void dbup_mysql_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(MySqlExtensions).Assembly)
                .ShouldMatchApproved(b => b
                    .WithFilenameGenerator((info, descriminator, type, extension) => $"DbUp.MySql.{type}.cs")
                    .SubFolder("ApprovalFiles"));
        }

        [Fact]
        public void dbup_postgres_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(PostgresqlExtensions).Assembly)
                .ShouldMatchApproved(b => b
                    .WithFilenameGenerator((info, descriminator, type, extension) => $"DbUp.Postgresql.{type}.cs")
                    .SubFolder("ApprovalFiles"));
        }

        [Fact]
        public void dbup_sqlce_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(SqlCeExtensions).Assembly)
                .ShouldMatchApproved(b => b
                    .WithFilenameGenerator((info, descriminator, type, extension) => $"DbUp.SqlCe.{type}.cs")
                    .SubFolder("ApprovalFiles"));
        }

        [Fact]
        public void dbup_sqlite_has_no_public_api_changes()
        {
            PublicApiGenerator.PublicApiGenerator
                .GetPublicApi(typeof(SQLiteExtensions).Assembly)
                .ShouldMatchApproved(b => b
                    .WithFilenameGenerator((info, descriminator, type, extension) => $"DbUp.SqLite.{type}.cs")
                    .SubFolder("ApprovalFiles"));
        }
    }
}
#endif