using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Transactions;
using Sap.Data.SQLAnywhere;

namespace DbUp.SqlAnywhere
{
    public class SqlAnywhereConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Split text on ; or GO and ignore those between Being and End statements
        /// </summary>
        static readonly Regex splitOnCommaOrGoRegEx = new Regex(@"\s*(?:(?<!BEGIN(?:.(?!END))*)(?:;|\n\s*GO\s*\n)(?!(?:.(?<!BEGIN))*END))\s*",
                                                                        RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public SqlAnywhereConnectionManager(string connectionString) : base(l => new SAConnection(connectionString))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var stringSeparators = new string[] { "GO" };
            var parts = scriptContents.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            return parts.Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
        }
    }
}