using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbUp.Helpers;
using DbUp.Support.SqlServer;

namespace DbUp.Engine.Preprocessors
{
    /// <summary>
    /// Substitutes variables for values in SqlScripts
    /// </summary>
    public class VariableSubstitutionPreprocessor : IScriptPreprocessor
    {
        private readonly IDictionary<string, string> variables;
        private static readonly Regex tokenRegex = new Regex(@"\$(?<variableName>\w+)\$");

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableSubstitutionPreprocessor"/> class.
        /// </summary>
        /// <param name="variables">The variables.</param>
        public VariableSubstitutionPreprocessor(IDictionary<string, string> variables)
        {
            this.variables = variables;
        }

        /// <summary>
        /// Substitutes variables 
        /// </summary>
        /// <param name="contents"></param>
        public string Process(string contents)
        {
            using (var parser = new VariableSubstitutionSqlParser(contents))
            {
                return parser.ReplaceVariables(variables);
            }
        }

        private class VariableSubstitutionSqlParser : SqlParser
        {
            public VariableSubstitutionSqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) 
                : base(sqlText, delimiter, delimiterRequiresWhitespace)
            {
            }

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

            protected override bool IsCustomStatement
            {
                get
                {
                    var c = PeekChar();
                    return CurrentChar == '$' 
                        && ValidVariableNameCharacter(c);
                }
            }

            private static bool ValidVariableNameCharacter(char c)
            {
                return (char.IsLetterOrDigit(c) || c == '_' || c == '-');
            }

            protected override void ReadCustomStatement()
            {
                var sb = new StringBuilder();
                while (Read() > 0 && CurrentChar != '$' && ValidVariableNameCharacter(CurrentChar))
                {
                    sb.Append(CurrentChar);
                }

                var buffer = sb.ToString();

                if (CurrentChar == '$' && ReadVariableName != null)
                {
                    ReadVariableName(buffer);
                }
                else
                {
                    OnReadCharacter(CharacterType.Command, '$');
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
}
