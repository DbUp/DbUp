using System;
using System.Text.RegularExpressions;
using DbUp.Engine;

namespace DbUp.SqlAnywhere
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with Sql Anywhere.
    /// </summary>
    public class SqlAnywhereSqlPreprocessor : IScriptPreprocessor
    {
        public string Process(string contents)
        {
            contents = Regex.Replace(contents, @"identity(\(?.*?\))", "identity", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return Regex.Replace(contents, "@", ":", RegexOptions.Singleline);
        }
    }
}
