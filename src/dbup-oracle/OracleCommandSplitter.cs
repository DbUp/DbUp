using System;
using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.Oracle
{
    public class OracleCommandSplitter
    {
        private readonly Func<string, SqlCommandReader> commandReaderFactory;

        [Obsolete]
        public OracleCommandSplitter()
        {
            this.commandReaderFactory = scriptContents => new OracleCommandReader(scriptContents);
        }
        
        public OracleCommandSplitter(char delimiter)
        {
            this.commandReaderFactory = scriptContents => new OracleCustomDelimiterCommandReader(scriptContents, delimiter);
        }
        
        /// <summary>
        /// Splits a script with multiple delimited commands into commands
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <returns></returns>
        public IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            using (var reader = commandReaderFactory(scriptContents))
            {
                var commands = new List<string>();
                reader.ReadAllCommands(c => commands.Add(c));
                return commands;
            }
        }
    }
}
