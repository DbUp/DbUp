using System;
using System.IO;

namespace DbUp.Support
{
    public abstract class SqlParser : StringReader
    {
        readonly string sqlText;
        int currentIndex;

        const char NullChar = (char)0;
        const char EndOfLineChar = '\n';
        const char CarriageReturn = '\r';
        const char SingleQuoteChar = (char)39;
        const char DashChar = '-';
        const char SlashChar = '/';
        const char StarChar = '*';
        const char OpenBracketChar = '[';
        const char CloseBracketChar = ']';

        protected const int FailedRead = -1;

        /// <summary>
        /// Creates a new instance of <see cref="SqlParser"/>.
        /// </summary>
        /// <param name="sqlText">The text to be read</param>
        /// <param name="delimiter">The command delimiter</param>
        /// <param name="delimiterRequiresWhitespace">Whether it requires whitespace</param>
        public SqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) : base(sqlText)
        {
            this.sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));
            Delimiter = delimiter;
            DelimiterRequiresWhitespace = delimiterRequiresWhitespace;
            currentIndex = -1;
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
            get => currentIndex;
            private set
            {
                currentIndex = value;
                LastChar = sqlText[currentIndex - 1];
                CurrentChar = sqlText[currentIndex];
            }
        }

        protected void Parse()
        {
            while (Read() != FailedRead)
            {
                if (IsCustomStatement)
                {
                    ReadCustomStatement();
                }
                else if (IsQuote)
                {
                    ReadQuotedString();
                }
                else if (IsBeginningOfBracketedText)
                {
                    ReadBracketedText();
                }
                else if (IsBeginningOfDashDashComment)
                {
                    ReadDashDashComment();
                }
                else if (IsBeginningOfSlashStarComment)
                {
                    ReadSlashStarComment();
                }
                else if (IsBeginningOfDelimiter)
                {
                    if (ReadDelimiter() && CommandEnded != null)
                        CommandEnded();
                }
                else
                {
                    ReadCharacter(CharacterType.Command, CurrentChar);
                }
            }
            CommandEnded?.Invoke();
        }

        #region Events to be subscribed to by consuming or deriving classes

        /// <summary>
        /// Notifies a command has finished reading
        /// </summary>
        protected event Action CommandEnded;

        /// <summary>
        /// Notifies a character has been read, signaling the current context to the subscriber.
        /// <example>
        /// While reading a quoted string, subscriber would receive `(CharacterType.QuotedString, 'a')`
        /// </example>
        /// </summary>
        protected event Action<CharacterType, char> ReadCharacter;

        /// <summary>
        /// Enables signaling of the `ReadCharacter` event from derived classes
        /// </summary>
        /// <param name="type">The character's type or where it belongs to</param>
        /// <param name="c">The character that was read</param>
        protected void OnReadCharacter(CharacterType type, char c)
        {
            ReadCharacter?.Invoke(type, c);
        }

        #endregion

        /// <summary>
        /// Read a custom statement until it's end is detected
        /// </summary>
        protected virtual void ReadCustomStatement() { }

        /// <summary>
        /// Hook to support custom statements
        /// </summary>
        protected virtual bool IsCustomStatement => false;

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
        protected bool HasReachedEnd => Peek() == -1;

        /// <summary>
        /// Is the current character end and of line character
        /// </summary>
        protected bool IsEndOfLine => CurrentChar == EndOfLineChar;

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
        protected bool IsQuote =>
                // TODO should match double?
                CurrentChar == SingleQuoteChar;

        /// <summary>
        /// Is current character WhiteSpace
        /// </summary>
        protected bool IsWhiteSpace => char.IsWhiteSpace(CurrentChar);

        /// <summary>
        /// Peek at the next character
        /// </summary>
        protected char PeekChar() => HasReachedEnd ? NullChar : (char)Peek();

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

        bool IsBeginningOfBracketedText => CurrentChar == OpenBracketChar;

        bool IsEndOfBracketedText => CurrentChar == CloseBracketChar;

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

        bool IsBeginningOfSlashStarComment => CurrentChar == SlashChar && Peek() == StarChar;

        bool IsBeginningOfDelimiter
        {
            get
            {
                var lastCharIsNullOrEmpty = char.IsWhiteSpace(LastChar) || LastChar == NullChar || !DelimiterRequiresWhitespace;
                var isCurrentCharacterStartOfDelimiter = IsCurrentCharEqualTo(Delimiter[0]);
                return lastCharIsNullOrEmpty
                       && isCurrentCharacterStartOfDelimiter
                       && TryPeek(Delimiter.Length - 1, out var result)
                       && string.Equals(result, Delimiter.Substring(1), StringComparison.OrdinalIgnoreCase);
            }
        }

        bool IsEndOfSlashStarComment => CurrentChar == StarChar && Peek() == SlashChar;


        /// <summary>
        /// Reads a quoted string to the end, including quotes
        /// </summary>
        void ReadQuotedString()
        {
            ReadCharacter(CharacterType.QuotedString, CurrentChar);
            while (Read() != FailedRead)
            {
                if (IsCustomStatement)
                {
                    ReadCustomStatement();
                    Read();
                }
                ReadCharacter(CharacterType.QuotedString, CurrentChar);
                if (IsQuote)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Reads [text] including brackets
        /// </summary>
        void ReadBracketedText()
        {
            ReadCharacter(CharacterType.BracketedText, CurrentChar);
            while (Read() != FailedRead)
            {
                if (IsCustomStatement)
                {
                    ReadCustomStatement();
                    Read();
                }
                ReadCharacter(CharacterType.BracketedText, CurrentChar);
                if (IsEndOfBracketedText)
                {
                    var peekChar = PeekChar();
                    // Close brackets within brackets are escaped with another
                    // Close bracket. e.g. [a[b]]c] => a[b]c
                    if (peekChar == CloseBracketChar)
                    {
                        Read();
                        ReadCharacter(CharacterType.BracketedText, CurrentChar);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Reads --comments including --
        /// </summary>
        void ReadDashDashComment()
        {
            // Writes the current dash.
            ReadCharacter(CharacterType.DashComment, CurrentChar);
            // Read until we hit the end of line.
            do
            {
                if (Read() == FailedRead)
                {
                    break;
                }
                ReadCharacter(CharacterType.DashComment, CurrentChar);
            }
            while (!IsEndOfLine);
        }

        void ReadSlashStarComment()
        {
            // We have entered this method because we've identified the "/*" pattern.
            // Write both characters here as technically they go together as a token.
            ReadCharacter(CharacterType.SlashStarComment, CurrentChar);
            Read();
            ReadCharacter(CharacterType.SlashStarComment, CurrentChar);

            // Read until we find a the ending of the slash star comment,
            // Or a nested slash star comment.
            while (Read() != FailedRead)
            {
                // Try to get out of the comment straight away
                if (IsEndOfSlashStarComment)
                {
                    // Write both characters of the "*/" token immediately then return
                    ReadCharacter(CharacterType.SlashStarComment, CurrentChar);
                    Read();
                    ReadCharacter(CharacterType.SlashStarComment, CurrentChar);
                    return;
                }

                if (IsBeginningOfSlashStarComment)
                {
                    // Nested comment found - using recursive call to read it then continue the loop.
                    ReadSlashStarComment();
                    continue;
                }

                ReadCharacter(CharacterType.SlashStarComment, CurrentChar);
            }
        }

        bool ReadDelimiter()
        {
            var previousChar = CurrentChar;
            var buffer = new char[Delimiter.Length - 1];

            // read the rest of the delimiter
            Read(buffer, 0, Delimiter.Length - 1);

            // Support terminator.
            var peekChar = PeekChar();
            char? terminator = null;
            if (peekChar == ';' || peekChar == NullChar)
            {
                // if NullChar read will not succeed and CurrentChar will not change
                if (Read() > 0)
                {
                    terminator = CurrentChar;
                }
            }
            // Check that the statement is indeed a GO and not text starting with Go
            // If it is not a go, add text to buffer and continue
            else if (!char.IsWhiteSpace(peekChar) && DelimiterRequiresWhitespace)
            {
                ReadCharacter(CharacterType.Command, previousChar);

                foreach (var @char in buffer)
                {
                    ReadCharacter(CharacterType.Command, @char);
                }

                return false;
            }

            // add the first char of the delimiter
            ReadCharacter(CharacterType.Delimiter, previousChar);

            // add the body of the delimiter
            foreach (var @char in buffer)
            {
                ReadCharacter(CharacterType.Delimiter, @char);
            }

            // add the ';' terminator, if present
            if (terminator.HasValue)
                ReadCharacter(CharacterType.Delimiter, terminator.Value);

            return true;
        }

        /// <summary>
        /// Types of characters to be read by consumers of the SqlParser
        /// </summary>
        public enum CharacterType
        {
            /// <summary>Character belongs to a command</summary>
            Command,
            /// <summary>Character belongs to a /* comment</summary>
            SlashStarComment,
            /// <summary>Character belongs to a -- comment</summary>
            DashComment,
            /// <summary>Character belongs to [bracketed] text</summary>
            BracketedText,
            /// <summary>Character belongs to "quoted" text</summary>
            QuotedString,
            /// <summary>Character belongs do a delimiter (like GO)</summary>
            Delimiter,
            /// <summary>Character is a custom statement (open for new implementation)</summary>
            CustomStatement,
        }
    }
}
