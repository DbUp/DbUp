using System.Collections.Generic;

namespace DbUp.Support
{
    /// <summary>
    /// Responsible for splitting SQL text into a list of commands.
    /// </summary>
    public class SqlCommandSplitter
    {
        /// <summary>
        /// Returns the separate executable SQL commands within the SQL script.
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            using (var reader = new SqlCommandReader(scriptContents))
            {
                var commands = new List<string>();
                reader.ReadAllCommands(c => commands.Add(c));
                return commands;
            }
        }
    }
}
