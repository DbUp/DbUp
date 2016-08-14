using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;

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
            Regex regexObj = new Regex(@"/\*(?>(?:(?!\*/|/\*).)*)(?>(?:/\*(?>(?:(?!\*/|/\*).)*)\*/(?>(?:(?!\*/|/\*).)*))*).*?\*/|--.*?\r?[\n]", RegexOptions.Singleline);
            Match commentMatch = regexObj.Match(contents);

            return tokenRegex.Replace(contents, match => ReplaceToken(match, variables, commentMatch));
        }

        private static bool IsInComment(Match commentMatch, Match variableMatch)
        {
            Match m = commentMatch;
            while (m.Success)
            {
                if (variableMatch.Index >= m.Index && variableMatch.Index <= m.Index + m.Length)
                {
                    return true;
                }

                m = m.NextMatch();
            }
            return false;
        }

        private static string ReplaceToken(Match match, IDictionary<string, string> variables, Match commentMatch)
        {
            var variableName = match.Groups["variableName"].Value;
            string replaceValue;

            if (!variables.TryGetValue(variableName, out replaceValue))
            {
                if (!IsInComment(commentMatch, match))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Variable {0} has no value defined", variableName));
                }
                else
                {
                    return match.Value;
                }
            }
            else
            {
                return replaceValue;
            }
        }
    }
}
