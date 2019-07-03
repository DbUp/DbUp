using System;
using System.Collections.Generic;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.Engine
{
    public interface IScriptFilter
    {
        IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<ExecutedSqlScript> executedScripts, ScriptNameComparer comparer);
    }
}