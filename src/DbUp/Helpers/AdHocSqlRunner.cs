using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Preprocessors;
using DbUp.Support.SqlServer;

namespace DbUp.Helpers
{
    /// <summary>
    /// A helper for executing SQL queries easily.
    /// </summary>
    public class AdHocSqlRunner
    {
        private readonly IScriptPreprocessor[] additionalScriptPreprocessors;
        private readonly Dictionary<string, string> variables = new Dictionary<string, string>();
        private readonly Func<IDbCommand> commandFactory;
        private readonly Func<bool> variablesEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocSqlRunner"/> class.
        /// </summary>
        /// <param name="commandFactory">The command factory.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="additionalScriptPreprocessors">The additional script preprocessors.</param>
        /// <remarks>Sets the <c>variablesEnabled</c> setting to <c>true</c>.</remarks>
        public AdHocSqlRunner(Func<IDbCommand> commandFactory, string schema, params IScriptPreprocessor[] additionalScriptPreprocessors)
            : this(commandFactory, schema, () => true, additionalScriptPreprocessors)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocSqlRunner"/> class.
        /// </summary>
        /// <param name="commandFactory">The command factory.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="variablesEnabled">Function indicating <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="additionalScriptPreprocessors">The additional script preprocessors.</param>
        public AdHocSqlRunner(Func<IDbCommand> commandFactory, string schema, Func<bool> variablesEnabled, params IScriptPreprocessor[] additionalScriptPreprocessors)
        {
            this.commandFactory = commandFactory;
            this.variablesEnabled = variablesEnabled;
            this.additionalScriptPreprocessors = additionalScriptPreprocessors;
            Schema = schema;
        }

        /// <summary>
        /// Adds a variable to be substituted on Adhoc script
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public AdHocSqlRunner WithVariable(string variableName, string value)
        {
            variables.Add(variableName, value);
            return this;
        }

        /// <summary>
        /// Database Schema, should be null if database does not support schemas
        /// </summary>
        public string Schema { get; set; }

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
            var result = 0;
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
                        using (var reader = command.ExecuteReader())
                        {
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
                        }
                    });

            return results;
        }

        private void Execute(string commandText, IEnumerable<Func<string, object>> parameters, Action<IDbCommand> executor)
        {
            commandText = Preprocess(commandText);
            using (var command = commandFactory())
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

                executor(command);
            }
        }

        private string Preprocess(string query)
        {
            if (string.IsNullOrEmpty(Schema))
                query = new StripSchemaPreprocessor().Process(query);
            if (!string.IsNullOrEmpty(Schema) && !variables.ContainsKey("schema"))
                variables.Add("schema", SqlObjectParser.QuoteSqlObjectName(Schema));
            if (variablesEnabled())
                query = new VariableSubstitutionPreprocessor(variables).Process(query);
            query = additionalScriptPreprocessors.Aggregate(query, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));
            return query;
        }
    }
}