using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using DbUp.Engine;
using DbUp.Engine.Preprocessors;

namespace DbUp.Helpers
{
    /// <summary>
    /// A helper for executing SQL queries easily.
    /// </summary>
    public class AdHocSqlRunner
    {
        readonly IScriptPreprocessor[] additionalScriptPreprocessors;
        readonly Dictionary<string, string> variables = new Dictionary<string, string>();
        readonly Func<IDbCommand> commandFactory;
        readonly Func<bool> variablesEnabled;
        readonly ISqlObjectParser sqlObjectParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocSqlRunner"/> class.
        /// </summary>
        /// <param name="commandFactory">The command factory.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="additionalScriptPreprocessors">The additional script preprocessors.</param>
        /// <remarks>Sets the <c>variablesEnabled</c> setting to <c>true</c>.</remarks>
        public AdHocSqlRunner(Func<IDbCommand> commandFactory, ISqlObjectParser sqlObjectParser, string schema, params IScriptPreprocessor[] additionalScriptPreprocessors)
            : this(commandFactory, sqlObjectParser, schema, () => true, additionalScriptPreprocessors)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocSqlRunner"/> class.
        /// </summary>
        /// <param name="commandFactory">The command factory.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="variablesEnabled">Function indicating <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="additionalScriptPreprocessors">The additional script preprocessors.</param>
        public AdHocSqlRunner(Func<IDbCommand> commandFactory, ISqlObjectParser sqlObjectParser, string schema, Func<bool> variablesEnabled, params IScriptPreprocessor[] additionalScriptPreprocessors)
        {
            this.commandFactory = commandFactory;
            this.sqlObjectParser = sqlObjectParser;
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
        public object ExecuteScalar(string query, params Expression<Func<string, object>>[] parameters)
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
        public int ExecuteNonQuery(string query, params Expression<Func<string, object>>[] parameters)
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
        /// Executes a select query or procedure. Note: does not support queries returning multiple columns with the same name, for example:
        /// select 1 as mycolumn, 2 as mycolumn
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public List<Dictionary<string, string>> ExecuteReader(string query, params Expression<Func<string, object>>[] parameters)
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
                            for (var i = 0; i < reader.FieldCount; i++)
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

        void Execute(string commandText, IEnumerable<Expression<Func<string, object>>> parameters, Action<IDbCommand> executor)
        {
            commandText = Preprocess(commandText);
            using (var command = commandFactory())
            {
                command.CommandText = commandText;

                foreach (var param in parameters)
                {
                    var key = param.Parameters[0].Name;
                    var value = param.Compile()(null);
                    var p = command.CreateParameter();
                    p.ParameterName = key;
                    p.Value = value;
                    command.Parameters.Add(p);
                }

                executor(command);
            }
        }

        string Preprocess(string query)
        {
            if (string.IsNullOrEmpty(Schema))
                query = new StripSchemaPreprocessor().Process(query);
            if (!string.IsNullOrEmpty(Schema) && !variables.ContainsKey("schema"))
                variables.Add("schema", sqlObjectParser.QuoteIdentifier(Schema));
            if (variablesEnabled())
                query = new VariableSubstitutionPreprocessor(variables).Process(query);
            query = additionalScriptPreprocessors.Aggregate(query, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));
            return query;
        }
    }
}
