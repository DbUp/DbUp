using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parsley;

namespace DbUp.Oracle
{
    public class OracleSqlParser
    {
        private bool inCodeBlock;

        private readonly List<string> commands = new List<string>();

        public IEnumerable<string> Commands { get { return commands; } }

        public OracleSqlParser(string sql)
        {
            try
            {
                Parse(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("OracleSqlParser was not able to parse the SQL!", ex);
            }
            
        }

        private void Parse(string sql)
        {   
            var commandBuilder = new StringBuilder();

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(sql).ToList().Where(t => t.Kind.Skippable == false);

            foreach (var token in tokens)
            {
                // Console.WriteLine($"{token.Kind.Name}: {token.Literal}");
                Analyze(token);
                
                if( (inCodeBlock && token.Kind.Name == "Slash") ||
                    (!inCodeBlock && token.Kind.Name == "Semicolon"))
                {
                    FinalizeStatement(token, commandBuilder);
                    // Console.WriteLine(commandBuilder.ToString());
                    commands.Add(commandBuilder.ToString().Trim());
                    commandBuilder.Clear();
                    inCodeBlock = false;
                }
                else
                {
                    if (token.Kind.Name == "Whitespace")
                    {
                        if(commandBuilder.Length > 1 &&  commandBuilder[commandBuilder.Length-1] != ' ')
                            commandBuilder.Append(' ');
                    }
                    else
                        commandBuilder.Append(token.Literal);
                }
            }
        }

        private void FinalizeStatement(Token currentToken, StringBuilder commandBuilder)
        {
            if (string.Compare(currentToken.Literal, "Slash", StringComparison.CurrentCultureIgnoreCase) == 0)
                commandBuilder.Append(";");
        }

        private void Analyze(Token token)
        {
            if (string.Compare(token.Literal, "BEGIN", StringComparison.CurrentCultureIgnoreCase) == 0)
                inCodeBlock = true;
        }
    }
}
