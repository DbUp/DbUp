using System.Text.RegularExpressions;
using DbUp.Engine;

namespace Odin.Database.DbUp.PreProcessors
{
    public class RoleCreateIfNotExistsPreProcessor : IScriptPreprocessor
    {
        private const string PATTERN = @"CREATE\s+ROLE\s\[?(?<rolename>.*)\]";
        private static Regex _regex = new Regex(PATTERN, RegexOptions.IgnoreCase);
        public string Process(string contents)
        {
            string match = _regex.Replace(contents, @"

IF DATABASE_PRINCIPAL_ID('${rolename}') IS NULL
$&");
           
            return match;
        }
    }
}