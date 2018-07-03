﻿using System;
using System.Data.SqlClient;
using System.Diagnostics;
using DbUp.Engine.Output;
using DbUp.Helpers;

namespace DbUp.SqlServer.Helpers
{
    /// <summary>
    /// Used to create databases that are deleted at the end of a unit test.
    /// </summary>
    public class TemporarySqlDatabase : IDisposable
    {
        private const string localSqlInstance = @"(local)";

        private readonly string connectionString;
        private readonly AdHocSqlRunner database;
        private readonly string databaseName;
        private readonly AdHocSqlRunner master;
        private readonly SqlConnection sqlConnection;
        private readonly SqlConnection masterSqlConnection;

		/// <summary>
        /// Creates new TemporarySqlDatabase against (local)
        /// </summary>
        public TemporarySqlDatabase(string name) : this(name, localSqlInstance) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySqlDatabase(string name, string instanceName) :
             this(new SqlConnectionStringBuilder($"Server={instanceName};Database={name};Trusted_connection=true;Pooling=false"))
        {
        }

        /// <summary>
        /// Creates a new <see cref="TemporarySqlDatabase"/> using the specified <see cref="SqlConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="connectionStringBuilder"><see cref="SqlConnectionStringBuilder"/> specifying which database to create.</param>
        public TemporarySqlDatabase(SqlConnectionStringBuilder connectionStringBuilder)
        {
            var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ToString()); //so we don't mangle the connectionStringBuilder coming in
            builder.Pooling = false; // make sure connection pooling is disabled so the connection is actually closed as expected

            // set the temporary database information
            databaseName = builder.InitialCatalog;
            connectionString = builder.ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            database = new AdHocSqlRunner(sqlConnection.CreateCommand, new SqlServer.SqlServerObjectParser(), "dbo", () => true);

            // set the master database information
            builder.InitialCatalog = "master";
            masterSqlConnection = new SqlConnection(builder.ToString());
            master = new AdHocSqlRunner(() => masterSqlConnection.CreateCommand(), new SqlServerObjectParser(), "dbo", () => true);
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return connectionString; }
        }

        /// <summary>
        /// Gets a tool to run ad-hoc SQL queries.
        /// </summary>
        /// <value>The ad hoc.</value>
        public AdHocSqlRunner AdHoc
        {
            get { return database; }
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        public void Create()
        {
            masterSqlConnection.Open();
            try
            {
                master.ExecuteNonQuery("drop database [" + databaseName + "]");
            }
            catch
            {
            }
            master.ExecuteNonQuery("create database [" + databaseName + "]");
            sqlConnection.Open();
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void Dispose()
        {
            sqlConnection.Close();
            master.ExecuteNonQuery("drop database [" + databaseName + "]");
            masterSqlConnection.Dispose();
        }

        /// <summary>
        /// Helper method to create a new <see cref="TemporarySqlDatabase"/> from a connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to that contains the information for the temporary sql database.</param>
        /// <returns>An instance of <see cref="TemporarySqlDatabase"/> that will use the connection string provided.</returns>
        public static TemporarySqlDatabase FromConnectionString(string connectionString)
            => new TemporarySqlDatabase(new SqlConnectionStringBuilder(connectionString));
    }
}