using System;
using DbUp.Builder;
using DbUp.SqlServer;

#if SUPPORTS_AZURE_AD

/// <summary>Configuration extension methods for Azure SQL Server.</summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the global:: namespace so that it is always available
// ReSharper disable CheckNamespace
#pragma warning disable CA1050 // Declare types in namespaces
public static class AzureSqlServerExtensions
{
    /// <summary>Creates an upgrader for Azure SQL Databases using Azure AD Integrated Security.</summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo' if <see langword="null" />.</param>
    /// <returns>A builder for a database upgrader designed for Azure SQL Server databases.</returns>
    public static UpgradeEngineBuilder AzureSqlDatabaseWithIntegratedSecurity(this SupportedDatabases supported, string connectionString, string schema)
    {
        return supported.SqlDatabase(new AzureSqlConnectionManager(connectionString), schema);
    }

    /// <summary>Creates an upgrader for Azure SQL Databases using Azure AD Integrated Security.</summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo' if <see langword="null" />.</param>
    /// <returns>A builder for a database upgrader designed for Azure SQL Server databases.</returns>
    public static UpgradeEngineBuilder AzureSqlDatabaseWithIntegratedSecurity(this SupportedDatabases supported, string connectionString, string schema, string resource)
    {
        return AzureSqlDatabaseWithIntegratedSecurity(supported, connectionString, schema, resource, null);
    }

    /// <summary>Creates an upgrader for Azure SQL Databases using Azure AD Integrated Security.</summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo' if <see langword="null" />.</param>
    /// <param name="resource">Resource to access. e.g. https://management.azure.com/.</param>
    /// <param name="tenantId">If not specified, default tenant is used. Managed Service Identity REST protocols do not accept tenantId, so this can only be used with certificate and client secret based authentication.</param>
    /// <param name="azureAdInstance">Specify a value for clouds other than the Public Cloud.</param>
    /// <returns>A builder for a database upgrader designed for Azure SQL Server databases.</returns>
    public static UpgradeEngineBuilder AzureSqlDatabaseWithIntegratedSecurity(this SupportedDatabases supported, string connectionString, string schema, string resource, string tenantId, string azureAdInstance = "https://login.microsoftonline.com/")
    {
        return supported.SqlDatabase(new AzureSqlConnectionManager(connectionString, resource, tenantId, azureAdInstance), schema);
    }
}
#pragma warning restore CA1050 // Declare types in namespaces

#endif
