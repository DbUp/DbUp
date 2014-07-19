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
        private const char NullChar = (char)0;
        private const int FailedRead = -1;

        private StringBuilder commandScriptBuilder;
        private StringBuilder tempBuilder;
        private char lastChar;
        private char currentChar;
        private bool isCheckingForSeperator = true;
        private bool foundGo;
        private bool foundLetterG;
    
        public SqlCommandReader(string sqlText)
            : base(sqlText)
        {
            commandScriptBuilder = new StringBuilder();
            tempBuilder = new StringBuilder();
        }     
       
        public void ReadAllCommands(Action<string> handleCommand)
        {
            // While we have successful reads,
            string commandText = string.Empty;
            while (this.Read() != FailedRead)
            {
                // Read the next command.
                if (!this.ReadCommand())
                {
                    // Still within same command.. keep reading..
                    continue;
                }
                // We detected a GO statement - return the current command text.
                // NOTE: We are trimming whitespace from the commands.
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
     
        public override int Read()
        {
            var result = base.Read();
            if (result != FailedRead)
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
                return 10 == this.CurrentChar;
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
                return 39 == this.CurrentChar;
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
                return NullChar;
            }
            return (char)this.Peek();
        }
     
        private bool ReadCommand()
        {
            if (this.IsQuote)
            {
                this.ReadQuotedString();
                // we don't seperate within quotes.
                return false;
            }
            if (this.IsBeginningOfDashDashComment)
            {
                return this.ReadDashDashComment();
            }
            if (!this.IsBeginningOfSlashStarComment)
            {
                return this.ReadNext();
            }
            this.ReadSlashStarComment();
            return false;
        }

        private bool IsBeginningOfDashDashComment
        {
            get
            {
                if (this.CurrentChar != '-')
                {
                    return false;
                }
                return this.Peek() == '-';
            }
        }

        private bool IsBeginningOfSlashStarComment
        {
            get
            {
                if (this.CurrentChar != '/')
                {
                    return false;
                }
                return this.Peek() == '*';
            }
        }

        private bool EndSlashStarComment
        {
            get
            {
                if (this.LastChar != '*')
                {
                    return false;
                }
                return this.CurrentChar == '/';
            }
        }
     
        private void Write(string text)
        {
            this.commandScriptBuilder.Append(text);
        }
     
        private void Write(char c)
        {
            this.commandScriptBuilder.Append(c);
        }
     
        private void WriteCurrent()
        {
            this.commandScriptBuilder.Append(this.CurrentChar);
        }

        private bool ReadDashDashComment()
        {
            // Read the entire line until the end of the comment.
            this.WriteCurrent();
            do
            {
                if (Read() == FailedRead)
                {
                    break;
                }
                this.WriteCurrent();
            }
            while (!this.IsEndOfLine);
            // further reading will need to be sensitive to seperators again...
            this.isCheckingForSeperator = true;
            return this.foundGo;
        }

        private void ReadSlashStarComment()
        {
            if (isCheckingForSeperator && this.foundGo)
            {
                throw new Exception("Incorrect syntax was encountered while parsing GO. Cannot have a slash star /* comment */ after a GO statement.");
            }

            if (this.ReadSlashStarCommentWithResult())
            {
                // need to check for seperators again.
                this.isCheckingForSeperator = true;
            }
        }

        private bool ReadSlashStarCommentWithResult()
        {
            this.WriteCurrent();
            while (this.Read() != FailedRead)
            {
                if (!this.IsBeginningOfSlashStarComment)
                {
                    this.WriteCurrent();
                    if (!this.EndSlashStarComment)
                    {
                        continue;
                    }
                    return true;
                }
                else
                {
                    this.ReadSlashStarCommentWithResult();
                }
            }
            return false;
        }

        private void ReadQuotedString()
        {
            WriteCurrent();
            while (this.Read() != NullChar)
            {
                WriteCurrent();
                if (!this.IsQuote)
                {
                    continue;
                }
                return;
            }
        }

        private bool ReadNext()
        {
            // If not currently checking for seperator, just capture the current characters until we reach the next line.
            if (!isCheckingForSeperator)
            {
                if (!IsEndOfLine)
                {
                    WriteCurrent();
                    return false;
                }
                // Once we are at the end of the line, we need to resume checking for seperator again on the next line.
                WriteCurrent();
                isCheckingForSeperator = true;
                return false;
            }

            if (IsEndOfLine)
            {
                if (this.foundGo)
                {
                    this.Reset();
                    return true;
                }
                // We reached the end of the line with no GO statement..    
                // Write the buffered string for this line to the current command builder.
                WriteCurrentCharToCommand();
                // We need to check for seperator again on the next line - but no need to split yet.
                this.isCheckingForSeperator = true;
                return false;
            }
            if (IsWhiteSpace)
            {
                // Preserve whitespace within sql command text.
                tempBuilder.Append(CurrentChar);
                // no need to split yet.
                return false;
            }

            // Can we detect a split?
            bool isG = IsCurrentCharEqualTo('g');
            bool isO = IsCurrentCharEqualTo('o');
            if (!isG && !isO)
            {
                // No need to split yet.. so write char to current command.
                this.WriteCurrentCharToCommand();
                this.isCheckingForSeperator = false;
                return false;
            }
            if (isO)
            {
                if (!IsLastCharEqualTo('g') || this.foundGo)
                {
                    // Write this character as we have not detected a go.
                    this.WriteCurrentCharToCommand();
                    this.isCheckingForSeperator = false;
                }
                else
                {
                    // We found a go statement.
                    this.foundGo = true;
                }
            }
            if (IsCurrentCharEqualTo('g'))
            {
                // Found a letter g, We note this in case the next letter is an o then we have found a go!
                if (this.foundLetterG || !char.IsWhiteSpace(LastChar) && LastChar != NullChar)
                {
                    this.WriteCurrentCharToCommand();
                    this.isCheckingForSeperator = false;
                    return false;
                }
                this.foundLetterG = true;
            }
            // If we have reached the end of the script and we found a GO then reset.
            if (HasReachedEnd && this.foundGo)
            {
                this.Reset();
                return true;
            }
            tempBuilder.Append(CurrentChar);
            return false;
        }

        private void WriteCurrentCharToCommand()
        {
            tempBuilder.Append(CurrentChar);
            this.Write(tempBuilder.ToString());
        }

        private void Reset()
        {
            this.foundGo = false;
            this.foundLetterG = false;
            tempBuilder = new StringBuilder();
        }

        private void ClearForNextCommand()
        {
            lastChar = NullChar;
            currentChar = NullChar;
            this.commandScriptBuilder = new StringBuilder();
        }
      
    }
}
