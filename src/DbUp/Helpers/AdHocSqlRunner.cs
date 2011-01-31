using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbUp.Helpers
{
    /// <summary>
    /// A helper for executing SQL queries easily.
    /// </summary>
    public class AdHocSqlRunner
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocSqlRunner"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdHocSqlRunner(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Executes a scalar query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public object ExecuteScalar(string query, params Func<string, object>[] parameters)
        {
            object result = null;
            Execute(query, parameters, 
                    command =>
                        {
                            result = command.ExecuteScalar();
                        });
            return result;
        }

        /// <summary>
        /// Executes a query that returns the number of records modified.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string query, params Func<string, object>[] parameters)
        {
            int result = 0;
            Execute(query, parameters,
                    command =>
                        {
                            result = command.ExecuteNonQuery();
                        });
            return result;
        }

        /// <summary>
        /// Executes a select query or procedure.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public List<Dictionary<string, string>> ExecuteReader(string query, params Func<string, object>[] parameters)
        {
            var results = new List<Dictionary<string, string>>();
            Execute(query, parameters,
                    command =>
                        {
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var line = new Dictionary<string, string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var name = reader.GetName(i);
                                    var value = reader.GetValue(i);
                                    value = value == DBNull.Value ? null : value.ToString();
                                    line.Add(name, (string)value);
                                }
                                results.Add(line);
                            }
                        });

            return results;
        }

        private void Execute(string commandText, Func<string, object>[] parameters, Action<IDbCommand> executor)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                foreach (var p in parameters)
                {
                    var key = p.Method.GetParameters()[0].Name;
                    var value = p(null);
                    command.Parameters.AddWithValue(key, value);
                }

                connection.Open();

                executor(command);
            }
        }
    }
}