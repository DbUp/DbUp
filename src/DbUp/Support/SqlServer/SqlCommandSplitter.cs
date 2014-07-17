using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DbUp.Support.SqlServer
{
    public class SqlCommandSplitter
    {
        public virtual IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            using (var reader = new SqlCommandReader(scriptContents))
            {
                var commands = new List<string>();
                reader.ReadAllCommands(c => commands.Add(c));
                return commands;
            }
        }

        public IEnumerable<string> GetCommandsOld(string scriptContents)
        {

            var scriptStatements =
         Regex.Split(scriptContents, "^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
             .Select(x => x.Trim())
             .Where(x => x.Length > 0)
             .ToArray();

            return scriptStatements;
        }                  

    }



  


}

