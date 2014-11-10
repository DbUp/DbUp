// Methods SplitScriptIntoCommands, BreakIntoStatements, BreakScriptIntoLines,
// FindLineNumber, and AdjustDelimiterEnd are from MySql .Net/Connector and
// are Copyright © 2004,2010, Oracle and/or its affiliates.  All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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
