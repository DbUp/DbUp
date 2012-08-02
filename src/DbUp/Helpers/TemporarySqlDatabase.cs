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

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySqlDatabase(string name)
        {
            databaseName = name;
            connectionString = string.Format("Server=(local)\\SQLEXPRESS;Database={0};Trusted_connection=true;Pooling=false", databaseName);
            database = new AdHocSqlRunner(( ) => new SqlConnection(connectionString), "dbo", () => true);

            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = "master";

            master = new AdHocSqlRunner(() => new SqlConnection(builder.ToString()), "dbo", () => true);
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
            try
            {
                master.ExecuteNonQuery("drop database [" + databaseName + "]");
            }
            catch
            {
            }
            master.ExecuteNonQuery("create database [" + databaseName + "]");
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void Dispose()
        {
            master.ExecuteNonQuery("drop database [" + databaseName + "]");
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