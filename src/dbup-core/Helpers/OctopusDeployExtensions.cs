using System;
using System.Linq;
using DbUp.Engine;

// ReSharper disable once CheckNamespace
namespace DbUp
{
    public static class OctopusDeployExtensions
    {
        public static void WriteExecutedScriptsToOctopusTaskSummary(this DatabaseUpgradeResult result)
        {
            Console.WriteLine("##octopus[stdout-highlight]");
            Console.WriteLine($"Ran {result.Scripts.Count()} script{(result.Scripts.Count() == 1 ? "" : "s")}");
            foreach (var script in result.Scripts)
                Console.WriteLine(script.Name);
            Console.WriteLine("##octopus[stdout-default]");
        }

    }
}