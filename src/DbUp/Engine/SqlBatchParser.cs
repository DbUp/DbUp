using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DbUp.Engine
{
    /// <summary>
    /// Splits scripts based on their batch separator and comments
    /// </summary>
    public class SqlBatchParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <param name="batchSeparator"></param>
        /// <returns></returns>
        public IEnumerable<string> SplitScriptBatches(string scriptContents, string batchSeparator)
        {
            var pattern = "^\\s*" + batchSeparator + "\\s*$";

            return Regex.Split(scriptContents, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

        }
         
    }
}