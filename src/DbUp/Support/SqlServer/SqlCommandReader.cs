using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DbUp.Support.SqlServer
{   

    /// <summary>
    /// Reads SQL commands from an underlying text stream.
    /// </summary>
    public class SqlCommandReader : StringReader
    {
        private const char nullChar = (char)0;
        private const char endOfLineChar = (char)10;
        private const char singleQuoteChar = (char)39;
        private const char dashChar = '-';
        private const char slashChar = '/';
        private const char starChar = '*';

        private const int failedRead = -1;
        private StringBuilder commandScriptBuilder;     
        private char lastChar;
        private char currentChar;             

        public SqlCommandReader(string sqlText)
            : base(sqlText)
        {
            commandScriptBuilder = new StringBuilder();          
        }

        public void ReadAllCommands(Action<string> handleCommand)
        {
            // While we have successful reads,
            string commandText = string.Empty;
            while (!HasReachedEnd)
            {
                // Read the command.
                this.ReadToEndOfCommand();
                // Remove unnecessary whitespace from command text before passing it back.
                commandText = commandScriptBuilder.ToString().Trim();
                if (commandText.Length > 0)
                {
                    handleCommand(commandText);
                    this.ClearForNextCommand();
                }
            }
            // We cannot read any more text. If we have content then this is the last (or only) command in the script, return it.
            if (commandScriptBuilder.Length > 0)
            {
                // NOTE: We are trimming whitespace from the commands.
                commandText = commandScriptBuilder.ToString().Trim();
                if (commandText.Length > 0)
                {
                    handleCommand(commandText);
                    this.ClearForNextCommand();
                }
            }

        }

        public void ReadToEndOfCommand()
        {
            //- Are we at the start of a single line comment - then read to end of line.
            //- Are we at the start of a multi line comment - then read to end of multiline comment.
            //- If not either of the above then we need to detect GO
            while (Read() != failedRead)
            {
                if (IsQuote)
                {
                    ReadQuotedString();
                    continue;
                }
                if (IsBeginningOfDashDashComment)
                {
                    ReadDashDashComment();
                    continue;
                }
                if (IsBeginningOfSlashStarComment)
                {
                    ReadSlashStarComment();
                    continue;
                }
                if (IsBeginningOfGo)
                {
                    ReadGo();
                    return;
                }
                else
                {
                    this.WriteCharToCommand();
                }
            }
        }

        public override int Read()
        {
            var result = base.Read();
            if (result != failedRead)
            {
                this.lastChar = this.currentChar;
                this.currentChar = (char)result;
                return result;
            }
            else
            {
                return result;
            }

        }

        protected char LastChar
        {
            get
            {
                return this.lastChar;
            }
        }

        protected char CurrentChar
        {
            get
            {
                return this.currentChar;
            }
        }

        protected bool HasReachedEnd
        {
            get
            {
                return this.Peek() == -1;
            }
        }

        protected bool IsEndOfLine
        {
            get
            {
                return this.CurrentChar == endOfLineChar;
            }
        }

        protected bool IsCurrentCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(this.CurrentChar, comparisonChar);
        }

        protected bool IsLastCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(this.LastChar, comparisonChar);
        }

        protected bool IsCharEqualTo(char comparisonChar, char compareTo)
        {
            return char.ToLowerInvariant(comparisonChar) == char.ToLowerInvariant(compareTo);
        }

        protected bool IsQuote
        {
            get
            {
                return this.CurrentChar == singleQuoteChar;
            }
        }

        protected bool IsWhiteSpace
        {
            get
            {
                return char.IsWhiteSpace(this.CurrentChar);
            }
        }

        protected char PeekChar()
        {
            if (this.HasReachedEnd)
            {
                return nullChar;
            }
            return (char)this.Peek();
        }   

        private bool IsBeginningOfDashDashComment
        {
            get
            {
                if (this.CurrentChar != dashChar)
                {
                    return false;
                }
                return this.Peek() == dashChar;
            }
        }

        private bool IsBeginningOfSlashStarComment
        {
            get
            {
                return this.CurrentChar == slashChar && this.Peek() == starChar;
            }
        }

        private bool IsBeginningOfGo
        {
            get
            {
                bool lastCharIsNullOrEmpty = Char.IsWhiteSpace(LastChar);
                bool currentCharIsG = IsCurrentCharEqualTo('g');
                bool nextCharIsO = IsCharEqualTo('o', this.PeekChar());
                return lastCharIsNullOrEmpty && currentCharIsG && nextCharIsO;
            }
        }

        private bool IsEndOfSlashStarComment
        {
            get
            {
                return this.LastChar == starChar && this.CurrentChar == slashChar;
            }
        }       

        private void WriteCharToCommand()
        {
            this.commandScriptBuilder.Append(this.CurrentChar);
        }

        private void ReadQuotedString()
        {
            WriteCharToCommand();
            while (this.Read() != failedRead)
            {
                WriteCharToCommand();
                if (this.IsQuote)
                {
                    return;
                }
            }
        }

        private void ReadDashDashComment()
        {
            // Writes the current dash.
            this.WriteCharToCommand();
            // Read until we hit the end of line.
            do
            {
                if (Read() == failedRead)
                {
                    break;
                }
                this.WriteCharToCommand();
            }
            while (!this.IsEndOfLine);
        }

        private void ReadSlashStarComment()
        {
            // Write the current slash.
            this.WriteCharToCommand();
            // Read until we find a the ending of the slash star comment,
            // Or a nested slash star comment.
            while (this.Read() != failedRead)
            {
                if (this.IsBeginningOfSlashStarComment)
                {
                    // Nested comment found - using recursive call to read it.
                    this.ReadSlashStarComment();
                }
                else
                {
                    this.WriteCharToCommand();
                    if (this.IsEndOfSlashStarComment)
                    {
                        return;
                    }
                    continue;
                }
            }
        }

        private void ReadGo()
        {
            // read to the o.
            if (Read() == failedRead)
            {
                return;
            }
            // Support terminator.
            if (this.PeekChar() == ';')
            {
                Read();
            }
        }        

        private void ClearForNextCommand()
        {
            lastChar = nullChar;
            currentChar = nullChar;
            commandScriptBuilder.Length = 0;          
        }

    }
}
