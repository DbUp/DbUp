using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DbUp.Support.SqlServer;

namespace DbUp.Engine.Preprocessors
{
    /// <summary>
    /// Parses the Sql and substitutes variables, used by the VariableSubstitutionPreprocessor
    /// </summary>
    public class VariableSubstitutionSqlParser : SqlParser
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sqlText">The sql to be parsed</param>
        /// <param name="delimiter">The command delimiter (default = "GO")</param>
        /// <param name="delimiterRequiresWhitespace">Whether the delimiter requires whitespace after it (default = true)</param>
        public VariableSubstitutionSqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true)
            : base(sqlText, delimiter, delimiterRequiresWhitespace)
        {
        }

        /// <summary>
        /// Delimiter character for variables.
        /// Defaults to `$` but can be overriden in derived classes.
        /// </summary>
        protected virtual char VariableDelimiter
        {
            get { return '$'; }
        }

        /// <summary>
        /// Replaces variables in the parsed SQL
        /// </summary>
        /// <param name="variables">Variable map</param>
        /// <returns>The sql with all variables replaced</returns>
        /// <exception cref="InvalidOperationException">Throws if a variable is present in the SQL but not in the `variables` map</exception>
        public string ReplaceVariables(IDictionary<string, string> variables)
        {
            var sb = new StringBuilder();

            this.ReadCharacter += (type, c) => sb.Append(c);

            this.ReadVariableName += (name) =>
            {
                if (!variables.ContainsKey(name))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Variable {0} has no value defined", name));
                }

                sb.Append(variables[name]);
            };

            this.Parse();

            return sb.ToString();
        }

        /// <summary>
        /// Checks if it's the beginning of a variable
        /// </summary>
        protected override bool IsCustomStatement
        {
            get
            {
                var c = PeekChar();
                return CurrentChar == VariableDelimiter
                       && ValidVariableNameCharacter(c);
            }
        }

        /// <summary>
        /// Verifies a character satisfies variable naming rules
        /// </summary>
        /// <param name="c">The character</param>
        /// <returns>True if it's a valid variable name character</returns>
        protected virtual bool ValidVariableNameCharacter(char c)
        {
            return (char.IsLetterOrDigit(c) || c == '_' || c == '-');
        }

        /// <summary>
        /// Read a variable for substitution
        /// </summary>
        protected override void ReadCustomStatement()
        {
            var sb = new StringBuilder();
            while (Read() > 0 && CurrentChar != VariableDelimiter && ValidVariableNameCharacter(CurrentChar))
            {
                sb.Append(CurrentChar);
            }

            var buffer = sb.ToString();

            if (CurrentChar == VariableDelimiter && ReadVariableName != null)
            {
                ReadVariableName(buffer);
            }
            else
            {
                OnReadCharacter(CharacterType.Command, VariableDelimiter);
                foreach (var c in buffer)
                {
                    OnReadCharacter(CharacterType.Command, c);
                }
                OnReadCharacter(CharacterType.Command, CurrentChar);
            }
        }

        private event Action<string> ReadVariableName;
    }
}