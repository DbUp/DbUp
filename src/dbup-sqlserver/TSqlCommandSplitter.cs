using System.Collections.Generic;
using System.IO;
using System.Text;
using DbUp.Support;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace DbUp.SqlServer
{
    public class TSqlCommandSplitter : SqlCommandSplitter
    {
        readonly TSql150Parser parser;

        public TSqlCommandSplitter()
        {
            parser = new TSql150Parser(true, SqlEngineType.All);
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            using (var scriptReader = new StringReader(scriptContents))
            {
                var parsedFragment = parser.Parse(scriptReader, out var errors);

                if (errors.Count > 0)
                {
                    throw new TSqlParseException($"Failure parsing SQL script \"{errors[0].Message}\" at [{errors[0].Line}, {errors[0].Column}]");
                }

                var batchBuilder = new StringBuilder();

                foreach (var token in parsedFragment.ScriptTokenStream)
                {
                    if (token.TokenType == TSqlTokenType.Go)
                    {
                        yield return batchBuilder.ToString().Trim();
                        batchBuilder = new StringBuilder();
                        continue;
                    }

                    batchBuilder.Append(token.Text);
                }

                yield return batchBuilder.ToString().Trim();
            }
        }
    }
}
