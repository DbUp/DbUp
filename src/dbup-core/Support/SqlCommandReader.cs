using System;
using System.IO;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace DbUp.Support
{
    /// <summary>
    /// Reads SQL commands from an underlying text stream.
    /// </summary>
    public class SqlCommandReader : StringReader
    {
        private readonly string sqlText;
        private readonly StringBuilder commandScriptBuilder;
        private int currentIndex;

        private const char NullChar = (char)0;
        private const char EndOfLineChar = '\n';
        private const char CarriageReturn = '\r';
        private const char SingleQuoteChar = (char)39;
        private const char DashChar = '-';
        private const char SlashChar = '/';
        private const char StarChar = '*';
        private const char OpenBracketChar = '[';
        private const char CloseBracketChar = ']';

        protected const int FailedRead = -1;

        /// <summary>
        /// Creates an instance of SqlCommandReader
        /// </summary>
        public SqlCommandReader(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true)
            : base(sqlText)
        {
            this.sqlText = sqlText;
            Delimiter = delimiter;
            DelimiterRequiresWhitespace = delimiterRequiresWhitespace;
            currentIndex = -1;
            commandScriptBuilder = new StringBuilder();
        }

        /// <summary>
        /// The current delimiter
        /// </summary>
        protected string Delimiter { get; set; }

        /// <summary>
        /// If true it indicates the delimiter requires whitespace before being used
        /// </summary>
        protected bool DelimiterRequiresWhitespace { get; set; }

        /// <summary>
        /// The index of the current character
        /// </summary>
        protected int CurrentIndex
        {
            get { return currentIndex; }
            private set
            {
                currentIndex = value;
                LastChar = sqlText[currentIndex - 1];
                CurrentChar = sqlText[currentIndex];
            }
        }

        /// <summary>
        /// Calls back for each command
        /// </summary>
        public void ReadAllCommands(Action<string> handleCommand)
        {
            while (!HasReachedEnd)
            {
                var commandText = ReadCommand();
                if (commandText.Length > 0)
                {
                    handleCommand(commandText);
                }
            }
        }

        private string ReadCommand()
        {
            // Command text in buffer start empty. 
            ResetCommandBuffer();

            while (Read() != FailedRead)
            {
                if (IsCustomStatement)
                {
                    ReadCustomStatement();
                    continue;
                }
                if (IsQuote)
                {
                    ReadQuotedString();
                    continue;
                }
                if (IsBeginningOfBracketedText)
                {
                    ReadBracketedText();
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
                if (IsBeginningOfDelimiter)
                {
                    if (!ReadDelimiter())
                        continue;
                    // This is the end of the command - return the command text in the buffer.
                    return GetCurrentCommandTextFromBuffer().Trim();
                }
                WriteCurrentCharToCommandTextBuffer();
            }
            return GetCurrentCommandTextFromBuffer().Trim();
        }

        /// <summary>
        /// Read a custom statement
        /// </summary>
        protected virtual void ReadCustomStatement()
        {
        }

        /// <summary>
        /// Hook to support custom statements
        /// </summary>
        protected virtual bool IsCustomStatement { get { return false; } }

        public override int Read()
        {
            var result = base.Read();
            if (result != FailedRead)
            {
                currentIndex++;
                LastChar = CurrentChar;
                CurrentChar = (char)result;
                // We don't care about Carriage returns, unless it is followed by End of line
                if (CurrentChar == CarriageReturn && PeekChar() != EndOfLineChar)
                    return Read();
                return result;
            }

            return result;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            var read = base.Read(buffer, index, count);
            CurrentIndex += read;
            CurrentChar = sqlText[CurrentIndex];
            return read;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            var read = base.ReadBlock(buffer, index, count);
            CurrentIndex += read;
            return read;
        }

        public override string ReadLine()
        {
            var readLine = base.ReadLine();
            if (readLine != null)
                CurrentIndex += readLine.Length;
            return readLine;
        }

        public override string ReadToEnd()
        {
            CurrentIndex = sqlText.Length - 1;
            return base.ReadToEnd();
        }

        /// <summary>
        /// The previous character
        /// </summary>
        protected char LastChar { get; private set; }

        /// <summary>
        /// The current character
        /// </summary>
        protected char CurrentChar { get; private set; }

        /// <summary>
        /// Has the Command Reader reached the end of the file
        /// </summary>
        protected bool HasReachedEnd
        {
            get
            {
                return Peek() == -1;
            }
        }

        /// <summary>
        /// Is the current character end and of line character
        /// </summary>
        protected bool IsEndOfLine
        {
            get
            {
                return CurrentChar == EndOfLineChar;
            }
        }

        /// <summary>
        /// Does current character match the character argument
        /// </summary>
        protected bool IsCurrentCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(CurrentChar, comparisonChar);
        }

        /// <summary>
        /// Is the previous character equal to argument
        /// </summary>
        protected bool IsLastCharEqualTo(char comparisonChar)
        {
            return IsCharEqualTo(LastChar, comparisonChar);
        }

        /// <summary>
        /// Are the arguments the same character, ignoring case
        /// </summary>
        protected bool IsCharEqualTo(char comparisonChar, char compareTo)
        {
            return char.ToLowerInvariant(comparisonChar) == char.ToLowerInvariant(compareTo);
        }

        /// <summary>
        /// Is character a single quote
        /// </summary>
        protected bool IsQuote
        {
            get
            {
                // TODO should match double?
                return CurrentChar == SingleQuoteChar;
            }
        }

        /// <summary>
        /// Is current character WhiteSpace
        /// </summary>
        protected bool IsWhiteSpace
        {
            get
            {
                return char.IsWhiteSpace(CurrentChar);
            }
        }

        /// <summary>
        /// Peek at the next character
        /// </summary>
        protected char PeekChar()
        {
            if (HasReachedEnd)
            {
                return NullChar;
            }
            return (char)Peek();
        }

        /// <summary>
        /// Peek at the next character
        /// </summary>
        protected bool TryPeek(int numberOfCharacters, out string result)
        {
            var currentIndex = CurrentIndex;
            if (currentIndex + numberOfCharacters >= sqlText.Length)
            {
                result = null;
                return false;
            }

            result = sqlText.Substring(CurrentIndex + 1, numberOfCharacters);
            return true;
        }

        bool IsBeginningOfBracketedText
        {
            get
            {
                return CurrentChar == OpenBracketChar;
            }
        }

        bool IsEndOfBracketedText
        {
            get
            {
                return CurrentChar == CloseBracketChar;
            }
        }

        bool IsBeginningOfDashDashComment
        {
            get
            {
                if (CurrentChar != DashChar)
                {
                    return false;
                }
                return Peek() == DashChar;
            }
        }

        bool IsBeginningOfSlashStarComment
        {
            get
            {
                return CurrentChar == SlashChar && Peek() == StarChar;
            }
        }

        bool IsBeginningOfDelimiter
        {
            get
            {
                var lastCharIsNullOrEmpty = char.IsWhiteSpace(LastChar) || LastChar == NullChar || !DelimiterRequiresWhitespace;
                var isCurrentCharacterStartOfDelimiter = IsCurrentCharEqualTo(Delimiter[0]);
                string result;
                return
                    lastCharIsNullOrEmpty &&
                    isCurrentCharacterStartOfDelimiter &&
                    TryPeek(Delimiter.Length - 1, out result) &&
                    string.Equals(result, Delimiter.Substring(1), StringComparison.OrdinalIgnoreCase);
            }
        }

        bool IsEndOfSlashStarComment
        {
            get
            {
                return LastChar == StarChar && CurrentChar == SlashChar;
            }
        }

        void ReadQuotedString()
        {
            WriteCurrentCharToCommandTextBuffer();
            while (Read() != FailedRead)
            {
                WriteCurrentCharToCommandTextBuffer();
                if (IsQuote)
                {
                    return;
                }
            }
        }

        void ReadBracketedText()
        {
            WriteCurrentCharToCommandTextBuffer();
            while (Read() != FailedRead)
            {
                WriteCurrentCharToCommandTextBuffer();
                if (IsEndOfBracketedText)
                {
                    var peekChar = PeekChar();
                    // Close brackets within brackets are escaped with another
                    // Close bracket. e.g. [a[b]]c] => a[b]c
                    if (peekChar == CloseBracketChar)
                    {
                        Read();
                        WriteCurrentCharToCommandTextBuffer();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        void ReadDashDashComment()
        {
            // Writes the current dash.
            WriteCurrentCharToCommandTextBuffer();
            // Read until we hit the end of line.
            do
            {
                if (Read() == FailedRead)
                {
                    break;
                }
                WriteCurrentCharToCommandTextBuffer();
            }
            while (!IsEndOfLine);
        }

        void ReadSlashStarComment()
        {
            // Write the current slash.
            WriteCurrentCharToCommandTextBuffer();
            // Read until we find a the ending of the slash star comment,
            // Or a nested slash star comment.
            while (Read() != FailedRead)
            {
                if (IsBeginningOfSlashStarComment)
                {
                    // Nested comment found - using recursive call to read it.
                    ReadSlashStarComment();
                }
                else
                {
                    WriteCurrentCharToCommandTextBuffer();
                    if (IsEndOfSlashStarComment)
                    {
                        return;
                    }
                }
            }
        }

        bool ReadDelimiter()
        {
            var currentChar = CurrentChar;
            var buffer = new char[Delimiter.Length - 1];
            // read the rest of the delimiter
            if (Read(buffer, 0, Delimiter.Length - 1) != Delimiter.Length - 1)
            {
                return true;
            }
            // Support terminator.
            var peekChar = PeekChar();
            if (peekChar == ';' || peekChar == '\0')
            {
                Read();
            }
            // Check that the statement is indeed a GO and not text starting with Go
            // If it is not a go, add text to buffer and continue
            else if (!char.IsWhiteSpace(peekChar) && DelimiterRequiresWhitespace)
            {
                commandScriptBuilder.Append(currentChar);
                commandScriptBuilder.Append(CurrentChar);
                return false;
            }

            return true;
        }

        void ResetCommandBuffer()
        {
            LastChar = NullChar;
            CurrentChar = NullChar;
            commandScriptBuilder.Length = 0;
        }

        void WriteCurrentCharToCommandTextBuffer()
        {
            commandScriptBuilder.Append(CurrentChar);
        }

        string GetCurrentCommandTextFromBuffer()
        {
            return commandScriptBuilder.ToString();
        }

    }
}
