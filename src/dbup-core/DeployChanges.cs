using DbUp.Builder;

namespace DbUp;

/// <summary>
/// A fluent builder for creating database upgraders.
/// </summary>
public static class DeployChanges
{
    /// <summary>
    /// Returns the databases supported by DbUp.
    /// </summary>
    public static SupportedDatabases To { get; } = new();
}
