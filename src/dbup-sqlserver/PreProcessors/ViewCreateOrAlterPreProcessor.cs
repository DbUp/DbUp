using System.Text.RegularExpressions;
using DbUp.Engine;

namespace Odin.Database.DbUp.PreProcessors
{
    public class ViewCreateOrAlterPreProcessor : IScriptPreprocessor
    {
        private const string PATTERN = @"(\s)?CREATE\s+VIEW";
        private static readonly Regex Regex = new Regex(PATTERN, RegexOptions.IgnoreCase);
        public string Process(string contents)
        {
            return Regex.Replace(contents, "CREATE OR ALTER VIEW");
        }
    }
}