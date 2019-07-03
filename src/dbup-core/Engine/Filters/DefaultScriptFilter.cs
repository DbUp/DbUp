using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Support;

namespace DbUp.Engine
{
    public class DefaultScriptFilter : IScriptFilter
    {
        public IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<ExecutedSqlScript> executedScripts, ScriptNameComparer comparer)
        {
            return sorted.Where(x =>
            {
                ExecutedSqlScript executedSqlScript = executedScripts
                    .FirstOrDefault(s => s.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

                //if it's a run always script
                //if the script has not been run (executedSqlScript ==null)
                //if the script's hashes do not match
                var willRun =  
                    x.SqlScriptOptions.ScriptType == ScriptType.RunAlways 
                    || executedSqlScript == null ||// ScriptType.RunOnce
                    (x.SqlScriptOptions.ScriptType == ScriptType.RunHash && executedSqlScript.Hash != x.Hash);
                return willRun;
            });

        }
    }
}