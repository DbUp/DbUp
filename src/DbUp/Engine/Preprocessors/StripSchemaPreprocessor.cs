using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace DbUp.Engine.Preprocessors
{
    /// <summary>
    /// Used for database engines that do not support schema's, it will remove $schema$. from all scripts
    /// </summary>
    public class StripSchemaPreprocessor : IScriptPreprocessor
    {

        private static readonly Regex dbUpStyleSchemaRegex = new Regex(@"\$schema\$\.", RegexOptions.IgnoreCase);
        private static readonly Regex sqlcmdStyleSchemaRegex = new Regex(@"\$\(schema\)\.", RegexOptions.IgnoreCase);

        /// <summary>
        /// Performs some proprocessing step on a script
        /// </summary>
        public string Process(string contents)
        {
            //return Regex.Replace(contents, @"\$schema\$\.", string.Empty, RegexOptions.IgnoreCase);

            return sqlcmdStyleSchemaRegex.Replace(dbUpStyleSchemaRegex.Replace(contents, string.Empty), string.Empty);
        }
    }
}
