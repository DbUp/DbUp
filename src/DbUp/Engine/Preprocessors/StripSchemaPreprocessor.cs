using System.Text.RegularExpressions;

namespace DbUp.Engine.Preprocessors
{
    /// <summary>
    /// Used for database engines that do not support schema's, it will remove $schema$. from all scripts
    /// </summary>
    public class StripSchemaPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some proprocessing step on a script
        /// </summary>
        public string Process(string contents)
        {
            return Regex.Replace(contents, @"\$schema\$\.", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}
