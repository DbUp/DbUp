using System;
using System.Collections.Generic;

namespace DbUp.Support;

/// <summary>
/// Compares script names using a provided comparer.
/// </summary>
public class ScriptNameComparer : IComparer<string>, IEqualityComparer<string>
{
    readonly IComparer<string> comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptNameComparer"/> class.
    /// </summary>
    /// <param name="comparer">The comparer to use for comparing script names.</param>
    public ScriptNameComparer(IComparer<string> comparer)
    {
        this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    /// <inheritdoc/>
    public int Compare(string x, string y) => comparer.Compare(x, y);

    /// <inheritdoc/>
    public bool Equals(string x, string y) => comparer.Compare(x, y) == 0;

    /// <inheritdoc/>
    public int GetHashCode(string obj) => obj.GetHashCode();
}
