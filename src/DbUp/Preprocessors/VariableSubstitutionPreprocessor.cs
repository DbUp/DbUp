using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DbUp.Preprocessors
{
    /// <summary>
    /// Substitutes variables for values in SqlScripts
    /// </summary>
    public class VariableSubstitutionPreprocessor : IScriptPreprocessor
    {
        private readonly IDictionary<string, string> variables;
        private static readonly Regex tokenRegex = new Regex(@"\$(?<variableName>\w+)\$");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variables"></param>
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
            return tokenRegex.Replace(contents, match => ReplaceToken(match, variables));
        }

        private static string ReplaceToken(Match match, IDictionary<string, string> variables)
        {
            var variableName = match.Groups["variableName"].Value;
            if (!variables.ContainsKey(variableName))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Variable {0} has no value defined", variableName));
            return variables[variableName];
        }
    }
}
