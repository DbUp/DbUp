using System.Collections.Generic;

namespace DbUp.Engine.Preprocessors
{
    /// <summary>
    /// Substitutes variables for values in SqlScripts
    /// </summary>
    public class VariableSubstitutionPreprocessor : IScriptPreprocessor
    {
        private readonly IDictionary<string, string> variables;

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
    }
}
