using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.Engine;

/// <summary>
/// Sorts the list of scripts before execution.
/// </summary>
public interface IScriptSorter
{
    /// <summary>
    /// Sorts the scripts into the order in which they should be executed.
    /// The resulting order does not have to be strict, but in such a case scripts that are considered equal may not run in a consistent order.
    /// </summary>
    IEnumerable<SqlScript> Sort(IEnumerable<SqlScript> scripts, ScriptNameComparer comparer);
}
