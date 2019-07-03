using System;
using System.Collections.Generic;
using DbUp.Engine;

namespace Odin.Database.DbUp.PreProcessors
{
    public class SqlCmdVariablePreProcessor : IScriptPreprocessor
    {
        private readonly IDictionary<string, string> _cmdVariables;

        public SqlCmdVariablePreProcessor(IDictionary<string, string> cmdVariables)
        {
            _cmdVariables = cmdVariables;
        }

        public string Process(string contents)
        {
            foreach (var cmdVariable in _cmdVariables)
            {
                string key = $"$({cmdVariable.Key})";
                
                if (contents.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    contents = contents.Replace(key, cmdVariable.Value, StringComparison.OrdinalIgnoreCase);
                }
            }

            return contents;
        }
    }
}