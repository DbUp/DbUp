// This class is an internal class from the MySql.Data.MySqlClient namespace
// of the MySql .Net Connector
 
using System;
using System.Collections.Generic;
using System.Text;

namespace DbUp.MySql
{
    internal class MySqlTokenizer
    {
        private string sql;
        private int startIndex;
        private int stopIndex;
        private bool ansiQuotes;
        private bool backslashEscapes;
        private bool returnComments;
        private bool multiLine;
        private bool sqlServerMode;
        private bool quoted;
        private bool isComment;
        private int pos;
        public string Text
        {
            get
            {
                return this.sql;
            }
            set
            {
                this.sql = value;
                this.pos = 0;
            }
        }
        public bool AnsiQuotes
        {
            get
            {
                return this.ansiQuotes;
            }
            set
            {
                this.ansiQuotes = value;
            }
        }
        public bool BackslashEscapes
        {
            get
            {
                return this.backslashEscapes;
            }
            set
            {
                this.backslashEscapes = value;
            }
        }
        public bool MultiLine
        {
            get
            {
                return this.multiLine;
            }
            set
            {
                this.multiLine = value;
            }
        }
        public bool SqlServerMode
        {
            get
            {
                return this.sqlServerMode;
            }
            set
            {
                this.sqlServerMode = value;
            }
        }
        public bool Quoted
        {
            get
            {
                return this.quoted;
            }
            private set
            {
                this.quoted = value;
            }
        }
        public bool IsComment
        {
            get
            {
                return this.isComment;
            }
        }
        public int StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                this.startIndex = value;
            }
        }
        public int StopIndex
        {
            get
            {
                return this.stopIndex;
            }
            set
            {
                this.stopIndex = value;
            }
        }
        public int Position
        {
            get
            {
                return this.pos;
            }
            set
            {
                this.pos = value;
            }
        }
        public bool ReturnComments
        {
            get
            {
                return this.returnComments;
            }
            set
            {
                this.returnComments = value;
            }
        }
        public MySqlTokenizer()
        {
            this.backslashEscapes = true;
            this.multiLine = true;
            this.pos = 0;
        }
        public MySqlTokenizer(string input)
            : this()
        {
            this.sql = input;
        }
        public List<string> GetAllTokens()
        {
            List<string> list = new List<string>();
            for (string item = this.NextToken(); item != null; item = this.NextToken())
            {
                list.Add(item);
            }
            return list;
        }
        public string NextToken()
        {
            if (!this.FindToken())
            {
                return null;
            }
            return this.sql.Substring(this.startIndex, this.stopIndex - this.startIndex);
        }
        public static bool IsParameter(string s)
        {
            return !string.IsNullOrEmpty(s) && (s[0] == '?' || (s.Length > 1 && s[0] == '@' && s[1] != '@'));
        }
        public string NextParameter()
        {
            while (this.FindToken())
            {
                if (this.stopIndex - this.startIndex >= 2)
                {
                    char c = this.sql[this.startIndex];
                    char c2 = this.sql[this.startIndex + 1];
                    if (c == '?' || (c == '@' && c2 != '@'))
                    {
                        return this.sql.Substring(this.startIndex, this.stopIndex - this.startIndex);
                    }
                }
            }
            return null;
        }
        public bool FindToken()
        {
            this.isComment = (this.quoted = false);
            this.startIndex = (this.stopIndex = -1);
            while (this.pos < this.sql.Length)
            {
                char c = this.sql[this.pos++];
                if (!char.IsWhiteSpace(c))
                {
                    if (c == '`' || c == '\'' || c == '"' || (c == '[' && this.SqlServerMode))
                    {
                        this.ReadQuotedToken(c);
                    }
                    else
                    {
                        if (c == '#' || c == '-' || c == '/')
                        {
                            if (!this.ReadComment(c))
                            {
                                this.ReadSpecialToken();
                            }
                        }
                        else
                        {
                            this.ReadUnquotedToken();
                        }
                    }
                    if (this.startIndex != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string ReadParenthesis()
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            int arg_11_0 = this.StartIndex;
            for (string text = this.NextToken(); text != null; text = this.NextToken())
            {
                stringBuilder.Append(text);
                if (text == ")" && !this.Quoted)
                {
                    return stringBuilder.ToString();
                }
            }
            throw new InvalidOperationException("Unable to parse SQL");
        }
        private bool ReadComment(char c)
        {
            if (c == '/' && (this.pos >= this.sql.Length || this.sql[this.pos] != '*'))
            {
                return false;
            }
            if (c == '-' && (this.pos + 1 >= this.sql.Length || this.sql[this.pos] != '-' || this.sql[this.pos + 1] != ' '))
            {
                return false;
            }
            string text = "\n";
            if (this.sql[this.pos] == '*')
            {
                text = "*/";
            }
            int num = this.pos - 1;
            int num2 = this.sql.IndexOf(text, this.pos);
            if (text == "\n")
            {
                num2 = this.sql.IndexOf('\n', this.pos);
            }
            if (num2 == -1)
            {
                num2 = this.sql.Length - 1;
            }
            else
            {
                num2 += text.Length;
            }
            this.pos = num2;
            if (this.ReturnComments)
            {
                this.startIndex = num;
                this.stopIndex = num2;
                this.isComment = true;
            }
            return true;
        }
        private void CalculatePosition(int start, int stop)
        {
            this.startIndex = start;
            this.stopIndex = stop;
            bool arg_14_0 = this.MultiLine;
        }
        private void ReadUnquotedToken()
        {
            this.startIndex = this.pos - 1;
            if (!this.IsSpecialCharacter(this.sql[this.startIndex]))
            {
                while (this.pos < this.sql.Length)
                {
                    char c = this.sql[this.pos];
                    if (char.IsWhiteSpace(c) || this.IsSpecialCharacter(c))
                    {
                        break;
                    }
                    this.pos++;
                }
            }
            this.Quoted = false;
            this.stopIndex = this.pos;
        }
        private void ReadSpecialToken()
        {
            this.startIndex = this.pos - 1;
            this.stopIndex = this.pos;
            this.Quoted = false;
        }
        private void ReadQuotedToken(char quoteChar)
        {
            if (quoteChar == '[')
            {
                quoteChar = ']';
            }
            this.startIndex = this.pos - 1;
            bool flag = false;
            bool flag2 = false;
            while (this.pos < this.sql.Length)
            {
                char c = this.sql[this.pos];
                if (c == quoteChar && !flag)
                {
                    flag2 = true;
                    break;
                }
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    if (c == '\\' && this.BackslashEscapes)
                    {
                        flag = true;
                    }
                }
                this.pos++;
            }
            if (flag2)
            {
                this.pos++;
            }
            this.Quoted = flag2;
            this.stopIndex = this.pos;
        }
        private bool IsQuoteChar(char c)
        {
            return c == '`' || c == '\'' || c == '"';
        }
        private bool IsParameterMarker(char c)
        {
            return c == '@' || c == '?';
        }
        private bool IsSpecialCharacter(char c)
        {
            return !char.IsLetterOrDigit(c) && c != '$' && c != '_' && c != '.' && !this.IsParameterMarker(c);
        }
    }
}