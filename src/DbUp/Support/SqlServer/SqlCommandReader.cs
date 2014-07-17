using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// Use to read the seperate SQL commands from an underlying text stream.
    /// </summary>
    public class SqlCommandReader : StringReader
    {
        private const char NullChar = (char)0;
        private const int FailedRead = -1;

        private StringBuilder _CommandScriptBuilder;
        private StringBuilder _TempBuilder;
        private char _LastChar;
        private char _CurrentChar;
        private bool _IsCheckingForSeperator = true;
        private bool _FoundGo;
        private bool _FoundLetterG;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sqlText">the sql text to read.</param>
        public SqlCommandReader(string sqlText)
            : base(sqlText)
        {
            _CommandScriptBuilder = new StringBuilder();
            _TempBuilder = new StringBuilder();
        }

        #region Public Methods

        /// <summary>
        /// Reads the commands from the stream.
        /// </summary>
        /// <param name="handleCommand">The callback for commands as they are read from the stream.</param>
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
                commandText = _CommandScriptBuilder.ToString().Trim();
                if (commandText.Length > 0)
                {
                    handleCommand(commandText);
                    this.ClearForNextCommand();                
                }
            }
            // We cannot read any more text. If we have content then this is the last (or only) command in the script, return it.
            if (_CommandScriptBuilder.Length > 0)
            {
                // NOTE: We are trimming whitespace from the commands.
                commandText = _CommandScriptBuilder.ToString().Trim();
                if (commandText.Length > 0)
                {
                    handleCommand(commandText);
                    this.ClearForNextCommand();                
                }
            }

        }

        /// <summary>
        /// Reads the next character from the sql script.
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            var result = base.Read();
            if (result != FailedRead)
            {
                this._LastChar = this._CurrentChar;
                this._CurrentChar = (char)result;
                return result;
            }
            else
            {
                return result;
            }

        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Returns the previous character that was read from the sql script.
        /// </summary>
        protected char LastChar
        {
            get
            {
                return this._LastChar;
            }
        }

        /// <summary>
        /// Returns the character at the current position in the sql script being read.
        /// </summary>
        protected char CurrentChar
        {
            get
            {
                return this._CurrentChar;
            }
        }

        /// <summary>
        /// Returns whether the end of the sql script has been reached.
        /// </summary>
        protected bool HasReachedEnd
        {
            get
            {
                return this.Peek() == -1;
            }
        }

        /// <summary>
        /// Returns whether the reader is at the end of a line or not.
        /// </summary>
        protected bool IsEndOfLine
        {
            get
            {
                return 10 == this.CurrentChar;
            }
        }

        /// <summary>
        /// Returns whether the curren char is equal to the char specified.
        /// </summary>
        /// <param name="comparisonChar"></param>
        /// <returns></returns>
        protected bool IsCurrentCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(this.CurrentChar, comparisonChar);
        }

        /// <summary>
        /// Returns whether the last char is equal to the char specified.
        /// </summary>
        /// <param name="comparisonChar"></param>
        /// <returns></returns>
        protected bool IsLastCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(this.LastChar, comparisonChar);
        }

        /// <summary>
        /// Returns whether the curren char is equal to the char specified.
        /// </summary>
        /// <param name="comparisonChar"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        protected bool IsCharEqualTo(char comparisonChar, char compareTo)
        {
            return char.ToLowerInvariant(comparisonChar) == char.ToLowerInvariant(compareTo);
        }

        /// <summary>
        /// Returns whether the current character is a Quote.
        /// </summary>
        protected bool IsQuote
        {
            get
            {
                return 39 == this.CurrentChar;
            }
        }

        /// <summary>
        /// Returns whether the current char is a whitespace.
        /// </summary>
        protected bool IsWhiteSpace
        {
            get
            {
                return char.IsWhiteSpace(this.CurrentChar);
            }
        }

        /// <summary>
        /// Returns the next char in the stream or a null character if its reached the end, without altering the current position.
        /// </summary>
        /// <returns></returns>
        protected char PeekChar()
        {
            if (this.HasReachedEnd)
            {
                return NullChar;
            }
            return (char)this.Peek();
        }

        /// <summary>
        /// Reads the next sql command from the stream, and returns whether a seperator was encountered.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Writes some text to output script.
        /// </summary>
        /// <param name="text"></param>
        private void Write(string text)
        {
            this._CommandScriptBuilder.Append(text);
        }

        /// <summary>
        /// Writes a character to the output script.
        /// </summary>
        /// <param name="c"></param>
        private void Write(char c)
        {
            this._CommandScriptBuilder.Append(c);
        }

        /// <summary>
        /// Writes the current character to the output script.
        /// </summary>     
        private void WriteCurrent()
        {
            this._CommandScriptBuilder.Append(this.CurrentChar);
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
            this._IsCheckingForSeperator = true;          
            return this._FoundGo;
        }

        private void ReadSlashStarComment()
        {
            if (_IsCheckingForSeperator && this._FoundGo)
            {
                throw new Exception("Incorrect syntax was encountered while parsing GO. Cannot have a slash star /* comment */ after a GO statement.");
            }

            if (this.ReadSlashStarCommentWithResult())
            {
                // need to check for seperators again.
                this._IsCheckingForSeperator = true;
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
            if (!_IsCheckingForSeperator)
            {
                if (!IsEndOfLine)
                {
                    WriteCurrent();
                    return false;
                }
                // Once we are at the end of the line, we need to resume checking for seperator again on the next line.
                WriteCurrent();
                _IsCheckingForSeperator = true;
                return false;
            }

            if (IsEndOfLine)
            {
                if (this._FoundGo)
                {
                    this.Reset();
                    return true;
                }
                // We reached the end of the line with no GO statement..    
                // Write the buffered string for this line to the current command builder.
                WriteCurrentCharToCommand();           
                // We need to check for seperator again on the next line - but no need to split yet.
                this._IsCheckingForSeperator = true;               
                return false;
            }
            if (IsWhiteSpace)
            {
                // Preserve whitespace within sql command text.
                _TempBuilder.Append(CurrentChar);
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
                this._IsCheckingForSeperator = false;   
                return false;
            }
            if (isO)
            {
                if (!IsLastCharEqualTo('g') || this._FoundGo)
                {
                    // Write this character as we have not detected a go.
                    this.WriteCurrentCharToCommand();
                    this._IsCheckingForSeperator = false;   
                }
                else
                {
                    // We found a go statement.
                    this._FoundGo = true;
                }
            }
            if (IsCurrentCharEqualTo('g'))
            {
                // Found a letter g, We note this in case the next letter is an o then we have found a go!
                if (this._FoundLetterG || !char.IsWhiteSpace(LastChar) && LastChar != NullChar)
                {
                    this.WriteCurrentCharToCommand();
                    this._IsCheckingForSeperator = false;   
                    return false;
                }
                this._FoundLetterG = true;
            }
            // If we have reached the end of the script and we found a GO then reset.
            if (HasReachedEnd && this._FoundGo)
            {
                this.Reset();
                return true;
            }
            _TempBuilder.Append(CurrentChar);
            return false;
        }

        private void WriteCurrentCharToCommand()
        {
            _TempBuilder.Append(CurrentChar);
            this.Write(_TempBuilder.ToString());                   
        }

        private void Reset()
        {
            this._FoundGo = false;
            this._FoundLetterG = false;
            _TempBuilder = new StringBuilder();
        }

        private void ClearForNextCommand()
        {           
            _LastChar = NullChar;
            _CurrentChar = NullChar;
            this._CommandScriptBuilder = new StringBuilder();
        }

        #endregion
    }
}
