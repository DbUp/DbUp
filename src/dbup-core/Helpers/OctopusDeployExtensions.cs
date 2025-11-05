using System;
using System.Linq;
using DbUp.Engine;

// ReSharper disable once CheckNamespace
namespace DbUp;

/// <summary>
/// Extension methods for integrating with Octopus Deploy.
/// </summary>
public static class OctopusDeployExtensions
{
    /// <summary>
    /// Writes executed scripts to Octopus Deploy task summary.
    /// </summary>
    /// <param name="result">The database upgrade result containing executed scripts.</param>
    public static void WriteExecutedScriptsToOctopusTaskSummary(this DatabaseUpgradeResult result)
    {
        Console.WriteLine("##octopus[stdout-highlight]");
        Console.WriteLine($"Ran {result.Scripts.Count()} script{(result.Scripts.Count() == 1 ? "" : "s")}");
        foreach (var script in result.Scripts)
            Console.WriteLine(script.Name);
        Console.WriteLine("##octopus[stdout-default]");
    }
}
