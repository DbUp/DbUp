using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using MySql.Data.MySqlClient;

namespace DbUp.MySql
{
    /// <summary>
    /// Manages MySql database connections.
    /// </summary>
    public class MySqlConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages MySql database connections.
        /// </summary>
        /// <param name="connectionString"></param>
        public MySqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new MySql database connection.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Splits the statements in the script using the ";" character
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <returns></returns>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var scriptStatements = BreakIntoStatements(true, true, ";", scriptContents)
                .Select(x => x.Text.Trim())
                .ToArray();

            return scriptStatements;
        }

        private IEnumerable<ScriptStatement> BreakIntoStatements(bool ansiQuotes, bool noBackslashEscapes, string delimiter, string query)
        {
            string text = delimiter;
            int num = 0;
            var list = new List<ScriptStatement>();
            List<int> list2 = BreakScriptIntoLines(query);
            var mySqlTokenizer = new MySqlTokenizer(query) {AnsiQuotes = ansiQuotes, BackslashEscapes = !noBackslashEscapes};

            for (string text2 = mySqlTokenizer.NextToken(); text2 != null; text2 = mySqlTokenizer.NextToken())
            {
                if (!mySqlTokenizer.Quoted)
                {
                    if (text2.ToLower(CultureInfo.InvariantCulture) == "delimiter")
                    {
                        mySqlTokenizer.NextToken();
                        AdjustDelimiterEnd(mySqlTokenizer, query);
                        text = query.Substring(mySqlTokenizer.StartIndex, mySqlTokenizer.StopIndex - mySqlTokenizer.StartIndex).Trim();
                        num = mySqlTokenizer.StopIndex;
                    }
                    else
                    {
                        if (text.StartsWith(text2, StringComparison.OrdinalIgnoreCase) && mySqlTokenizer.StartIndex + text.Length <= query.Length && query.Substring(mySqlTokenizer.StartIndex, text.Length) == text)
                        {
                            text2 = text;
                            mySqlTokenizer.Position = mySqlTokenizer.StartIndex + text.Length;
                            mySqlTokenizer.StopIndex = mySqlTokenizer.Position;
                        }
                        int num2 = text2.IndexOf(text, StringComparison.OrdinalIgnoreCase);
                        if (num2 != -1)
                        {
                            int num3 = mySqlTokenizer.StopIndex - text2.Length + num2;
                            if (mySqlTokenizer.StopIndex == query.Length - 1)
                            {
                                num3++;
                            }
                            string text3 = query.Substring(num, num3 - num);
                            ScriptStatement item = default(ScriptStatement);
                            item.Text = text3.Trim();
                            item.Line = FindLineNumber(num, list2);
                            item.Position = num - list2[item.Line];
                            list.Add(item);
                            num = num3 + text.Length;
                        }
                    }
                }
            }
            if (num < query.Length - 1)
            {
                string text4 = query.Substring(num).Trim();
                if (!string.IsNullOrEmpty(text4))
                {
                    ScriptStatement item2 = default(ScriptStatement);
                    item2.Text = text4;
                    item2.Line = FindLineNumber(num, list2);
                    item2.Position = num - list2[item2.Line];
                    list.Add(item2);
                }
            }
            return list;
        }

        private List<int> BreakScriptIntoLines(string query)
        {
            var list = new List<int>();
            var stringReader = new StringReader(query);
            string text = stringReader.ReadLine();
            int num = 0;
            while (text != null)
            {
                list.Add(num);
                num += text.Length;
                text = stringReader.ReadLine();
            }
            return list;
        }

        private static int FindLineNumber(int position, List<int> lineNumbers)
        {
            int num = 0;
            while (num < lineNumbers.Count && position < lineNumbers[num])
            {
                num++;
            }
            return num;
        }

        private void AdjustDelimiterEnd(MySqlTokenizer tokenizer, string query)
        {
            if (tokenizer.StopIndex < query.Length)
            {
                int num = tokenizer.StopIndex;
                char c = query[num];
                while (!char.IsWhiteSpace(c) && num < query.Length - 1)
                {
                    c = query[++num];
                }
                tokenizer.StopIndex = num;
                tokenizer.Position = num;
            }
        }
    }
}
