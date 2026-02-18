namespace DbUp.Support;

/// <summary>
/// Options for processing SQL object names.
/// </summary>
public enum ObjectNameOptions
{
    /// <summary>
    /// No options are set.
    /// </summary>
    None,

    /// <summary>
    /// Remove starting and ending white space from the object name.
    /// </summary>
    Trim
}
