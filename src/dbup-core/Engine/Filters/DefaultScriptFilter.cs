using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Helpers;
using DbUp.Support;

namespace DbUp.Engine.Filters
{
    public class DefaultScriptFilter : IScriptFilter
    {
        public IEnumerable<SqlScript> Filter(
            IEnumerable<SqlScript> sorted,
            HashSet<string> executedScriptNames,
            ScriptNameComparer comparer)
        {
            return sorted.Where(sqlScript => DefaultScriptFilter.ScriptShouldRun(sqlScript, executedScriptNames, comparer, null, null));
        }

        public IEnumerable<SqlScript> Filter(
            IOrderedEnumerable<SqlScript> sorted,
            IEnumerable<ExecutedSqlScript> executedScripts,
            ScriptNameComparer comparer,
            IHasher hasher)
        {
            var executedScriptsList = executedScripts.ToList();
            var executedScriptNames = new HashSet<string>(executedScriptsList.Select(x => x.Name), comparer);

            return sorted.Where(sqlScript => DefaultScriptFilter.ScriptShouldRun(sqlScript, executedScriptNames, comparer, executedScriptsList, hasher));
        }

        private static bool ScriptShouldRun(
            SqlScript sqlScript,
            IEnumerable<string> executedScriptNames,
            ScriptNameComparer comparer,
            IEnumerable<ExecutedSqlScript> executedScripts,
            IHasher hasher)
        {
            switch (sqlScript.SqlScriptOptions.ScriptType)
            {
                case ScriptType.RunAlways:
                    return true;
                case ScriptType.RunOnce:
                    return !executedScriptNames.Contains(sqlScript.Name, comparer);
                case ScriptType.RunOnChange:
                {
                    if (executedScripts == null)
                    {
                        throw new ArgumentNullException(nameof(executedScripts));
                    }

                    if (hasher == null)
                    {
                        throw new ArgumentNullException(nameof(hasher));
                    }

                    return DefaultScriptFilter.ScriptIsNewOrChanged(sqlScript, comparer, executedScripts, hasher);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool ScriptIsNewOrChanged(
            SqlScript sqlScript,
            ScriptNameComparer comparer,
            IEnumerable<ExecutedSqlScript> executedScripts,
            IHasher hasher) =>
            !executedScripts.Any(
                executedScript =>
                {
                    if (comparer.Equals(executedScript.Name, sqlScript.Name))
                    {
                        return executedScript.Hash == null || executedScript.Hash == hasher.GetHash(sqlScript.Contents);
                    }

                    return false;
                });
    }
}
