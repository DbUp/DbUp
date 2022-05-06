using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Support;

namespace DbUp.Spanner
{
    public class SpannerCommandSplitter
    {
        private readonly Func<string, SqlCommandReader> commandReaderFactory;

        public SpannerCommandSplitter()
        {
            this.commandReaderFactory = scriptContents => new SpannerCommandReader(scriptContents);
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
