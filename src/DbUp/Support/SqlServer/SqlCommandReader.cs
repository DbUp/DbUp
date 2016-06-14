using System;
using System.IO;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// Reads SQL commands from an underlying text stream.
    /// </summary>
    public class SqlCommandReader : SqlParser
    {
        private readonly StringBuilder commandScriptBuilder;

        protected const int FailedRead = -1;

        /// <summary>
        /// Creates an instance of SqlCommandReader
        /// </summary>
        public SqlCommandReader(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true)
            : base(sqlText, delimiter, delimiterRequiresWhitespace)
        {
            commandScriptBuilder = new StringBuilder();
        }

        /// <summary>
        /// Calls back for each command
        /// </summary>
        public void ReadAllCommands(Action<string> handleCommand)
        {
            while (!HasReachedEnd)
            {
                this.ReadCharacter += (type, c) =>
                {
                    switch (type)
                    {
                        case CharacterType.Command:
                        case CharacterType.SlashStarComment:
                        case CharacterType.DashComment:
                        case CharacterType.BracketedText:
                        case CharacterType.QuotedString:
                        case CharacterType.CustomStatement:
                            commandScriptBuilder.Append(c);
                            break;
                        case CharacterType.Delimiter:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("type", type, null);
                    }
                    
                };
                this.CommandEnded += () =>
                {
                    var commandText = GetCurrentCommandTextFromBuffer();
                    if (commandText.Length > 0)
                    {
                        handleCommand(commandText);
                        ResetCommandBuffer();
                    }
                };

                Parse();
            }
        }

        private void ResetCommandBuffer()
        {
            commandScriptBuilder.Length = 0;
        }

        private string GetCurrentCommandTextFromBuffer()
        {
            return commandScriptBuilder.ToString().Trim();
        }
    }
}
