using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Helpers;

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
            List<KeyValuePair<int, int>> comments = SqlCommentRangeFinder.FindRanges(contents);
            return tokenRegex.Replace(contents, match => ReplaceToken(match, variables, comments));
        }

        private static string ReplaceToken(Match match, IDictionary<string, string> variables, List<KeyValuePair<int, int>> comments)
        {
            var variableName = match.Groups["variableName"].Value;
            string replaceValue;

            if (!variables.TryGetValue(variableName, out replaceValue))
            {
                if (!IsInComment(comments, match))
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

        private static bool IsInComment(List<KeyValuePair<int, int>> commentRanges, Match match)
        {
            return commentRanges
                .TakeWhile(x => x.Key < match.Index)
                .Any(x => x.Key < match.Index && x.Value > (match.Index + match.Length));
        }
    }
}
