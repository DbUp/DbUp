using System.Text.RegularExpressions;
using DbUp.Engine;

namespace Odin.Database.DbUp.PreProcessors
{
    public class StoredProcedureCreateOrAlterPreProcessor : IScriptPreprocessor
    {
        private const string PATTERN = @"CREATE\s+PROC(EDURE)?";
        private static Regex _regex = new Regex(PATTERN, RegexOptions.IgnoreCase);
        public string Process(string contents)
        {
            return _regex.Replace(contents,  "CREATE OR ALTER PROCEDURE");
        }
    }
}