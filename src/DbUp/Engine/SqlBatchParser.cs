using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DbUp.Engine
{
    /// <summary>
    ///     Splits scripts based on their batch separator and comments
    /// </summary>
    public class SqlBatchParser
    {
        /// <summary>
        ///     Parses SQL Scripts based on batch separators (i.e. GO or ; and removes comments)
        /// </summary>
        /// <param name="scriptContents">SQL Script Body</param>
        /// <param name="batchSeparator">GO or ;</param>
        /// <returns></returns>
        public IEnumerable<string> SplitScriptBatches(string scriptContents, string batchSeparator)
        {
            scriptContents = StripComments(scriptContents);

            string pattern = "^\\s*" + batchSeparator + "\\s*$";

            return Regex.Split(scriptContents, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();
        }

        private string StripComments(string scriptContents)
        {
            const string commentPattern = @"/\*(.*?)\*/";

            return Regex.Replace(scriptContents, commentPattern, "", RegexOptions.Singleline);
        }
    }
}