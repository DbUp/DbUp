using System;
using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.Engine.Sorters;

/// <summary>
/// Script sorter that uses a delegate to order scripts. The comparer is not passed to the delegate;
/// use <see cref="DefaultScriptSorter"/> or a custom <see cref="IScriptSorter"/> if you need it.
/// </summary>
public class DelegateScriptSorter : IScriptSorter
{
    readonly Func<IEnumerable<SqlScript>, IEnumerable<SqlScript>> sortFunction;

    /// <summary>
    /// Creates a sorter that orders scripts using the given function.
    /// </summary>
    /// <param name="sortFunction">Function that returns scripts in the order they should be executed. The resulting order does not have to be strict, but in such a case scripts that are considered equal may not run in a consistent order.</param>
    public DelegateScriptSorter(Func<IEnumerable<SqlScript>, IEnumerable<SqlScript>> sortFunction)
    {
        this.sortFunction = sortFunction ?? throw new ArgumentNullException(nameof(sortFunction));
    }

    /// <inheritdoc />
    public IEnumerable<SqlScript> Sort(IEnumerable<SqlScript> scripts, ScriptNameComparer comparer)
        => sortFunction(scripts);
}
