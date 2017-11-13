using System.Collections.Generic;
using Parsley;

namespace DbUp.Oracle
{
    public class OracleSqlLexer : Lexer
    {
        private static List<TokenKind> Kinds
        {
            get
            {
                var rList = new List<TokenKind>
                {
                    new Pattern("BlockComment", @"/\*(?>(?:(?>[^*]+)|\*(?!/))*)\*/", true),
                    new Pattern("LineComment", @"--.*$", true),
                    new Pattern("Semicolon", @";"),
                    new Pattern("Wildcard", @"\*"),
                    new Pattern("DoubleQuoted", "\\\"(.*?)\\\""),
                    new Pattern("SingleQuoted", @"'([^']*)'"),
                    new Pattern("Word", @"[a-zA-Z0-9_\-]+"),
                    new Pattern("Equals", @"="),
                    new Pattern("Punctuation", "[\\[\\]{}(),:|`.]"),
                    new Pattern("Whitespace", @"\s+"),
                    new Pattern("Slash", @"/")
                };
                return rList;
            }
        }

        public IEnumerable<Token> Tokens { get; } = new List<Token>();

        public OracleSqlLexer()
            : base(Kinds.ToArray())
        {
        }
    }
}