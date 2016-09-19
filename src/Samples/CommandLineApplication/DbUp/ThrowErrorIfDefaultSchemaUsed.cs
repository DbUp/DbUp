using System;
using System.Text.RegularExpressions;
using DbUp.Engine;

namespace CommandLineApplication.DbUp
{
    /// <summary>Prevent scripts from using [dbo] schema</summary>
    public class ThrowErrorIfDefaultSchemaUsed : IScriptPreprocessor
    {
        public string Process(string contents)
        {
            if (Regex.IsMatch(contents, @"\[dbo\]", RegexOptions.IgnoreCase))
                throw new Exception("Cannot use default schema [dbo] in scripts as it is protected");

            return contents;
        }
    }
}
