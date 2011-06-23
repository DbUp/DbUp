using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Preprocessors;

namespace DbUp.Helpers
{
    /// <summary>
    /// A helper for executing SQL queries easily.
    /// </summary>
    public class AdHocSqlRunner
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly string schema;
        private readonly IScriptPreprocessor[] additionalScriptPreprocessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocSqlRunner"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="additionalScriptPreprocessors">The additional script preprocessors.</param>
        public AdHocSqlRunner(Func<IDbConnection> connectionFactory, string schema, params IScriptPreprocessor[] additionalScriptPreprocessors)
        {
            this.connectionFactory = connectionFactory;
            this.schema = schema;
            this.additionalScriptPreprocessors = additionalScriptPreprocessors;
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
            query = Preprocess(query);
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
            var result = 0;
            query = Preprocess(query);
            Execute(query, parameters,
                    command =>
                        {
                            result = command.ExecuteNonQuery();
                        });
            return result;
        }

        private string Preprocess(string query)
        {
            var variables = new Dictionary<string, string> {{"schema", schema}};
            query = new VariableSubstitutionPreprocessor(variables).Process(query);
            query = additionalScriptPreprocessors.Aggregate(query, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));
            return query;
        }

        /// <summary>
        /// Executes a select query or procedure.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public List<Dictionary<string, string>> ExecuteReader(string query, params Func<string, object>[] parameters)
        {
            query = Preprocess(query);
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

        private void Execute(string commandText, IEnumerable<Func<string, object>> parameters, Action<IDbCommand> executor)
        {
            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                foreach (var param in parameters)
                {
                    var key = param.Method.GetParameters()[0].Name;
                    var value = param(null);
                    var p = command.CreateParameter();
                    p.ParameterName = key;
                    p.Value = value;
                    command.Parameters.Add(p);
                }

                connection.Open();

                executor(command);
            }
        }
    }
}