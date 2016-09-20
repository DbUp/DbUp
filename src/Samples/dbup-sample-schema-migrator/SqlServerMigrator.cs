using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Sample.SchemaMigrator.DbUp;
using DbUp.Sample.SchemaMigrator.Migrations;

namespace DbUp.Sample.SchemaMigrator
{
    public sealed class SqlServerMigrator
    {
        public SqlServerMigrator(string connectionString, bool stableScriptsOnly)
        {
            _connectionString = connectionString;
            _stableScriptsOnly = stableScriptsOnly;
            _scriptNamespace = typeof(MigrationsPlaceholder).Namespace;
            _upgradeLogger = new ConsoleUpgradeLog();
        }

        public void Create()
        {
            EnsureDatabase.For.SqlDatabase(_connectionString, _upgradeLogger);
        }

        public bool IsUpgradeRequired()
        {
            var upgrader = DeployChanges.To
                .SqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(SqlServerMigrator).Assembly, _scriptNamespace, _stableScriptsOnly)
                .JournalToSqlTable("dbo", "SchemaMigrationHistory")
                .LogTo(_upgradeLogger)
                .Build();

            return upgrader.IsUpgradeRequired();
        }

        public DatabaseUpgradeResult PerformUpgrade()
        {
            var upgrader = DeployChanges.To
                .SqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(SqlServerMigrator).Assembly, _scriptNamespace, _stableScriptsOnly)
                .WithPreprocessor(new ThrowErrorIfDefaultSchemaUsed())
                .JournalToSqlTable("dbo", "SchemaMigrationHistory")
                .LogTo(_upgradeLogger)
                .WithTransaction()
                .Build();

            _upgradeLogger.WriteInformation("Connecting to {connectionString}", _connectionString);

            return upgrader.PerformUpgrade();
        }

        private readonly string _connectionString;
        private readonly bool _stableScriptsOnly;
        private readonly string _scriptNamespace;
        private readonly IUpgradeLog _upgradeLogger;
    }
}
