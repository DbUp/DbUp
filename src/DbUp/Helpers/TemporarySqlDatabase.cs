using System;
using System.Data.SqlClient;
using System.Diagnostics;
using DbUp.Engine.Output;

namespace DbUp.Helpers
{
    /// <summary>
    /// Used to create databases that are deleted at the end of a unit test.
    /// </summary>
    public class TemporarySqlDatabase : IDisposable
    {
        private readonly string connectionString;
        private readonly AdHocSqlRunner database;
        private readonly string databaseName;
        private readonly AdHocSqlRunner master;
        private readonly SqlConnection sqlConnection;
        private readonly SqlConnection masterSqlConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySqlDatabase(string name)
        {
            databaseName = name;
            connectionString = string.Format("Server=(local);Database={0};Trusted_connection=true;Pooling=false", databaseName);
            sqlConnection = new SqlConnection(connectionString);
            database = new AdHocSqlRunner(sqlConnection.CreateCommand, "dbo", () => true);

            var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };

            masterSqlConnection = new SqlConnection(builder.ToString());
            master = new AdHocSqlRunner(() => masterSqlConnection.CreateCommand(), "dbo", () => true);
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
            master.ExecuteNonQuery("drop database [" + databaseName + "]");
            masterSqlConnection.Dispose();
            sqlConnection.Close();
        }

        internal class TraceLog : IUpgradeLog
        {
            public void WriteInformation(string format, params object[] args)
            {
                Trace.TraceInformation(format, args);
            }

            public void WriteError(string format, params object[] args)
            {
                Trace.TraceError(format, args);
            }

            public void WriteWarning(string format, params object[] args)
            {
                Trace.TraceWarning(format, args);
            }

            public IDisposable Indent()
            {
                return new FooDisposable();
            }


            public class FooDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}