using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp
{
    /// <summary>
    /// A factory class for filter methods.
    /// </summary>
    // NOTE: DELIBRATELY LEFT IN THE ROOT NAMESPACE
    // Since this class is a helper class that is designed to be available when working with the DbUp Fluent API, 
    // we leave it in the root so that people don't have to manually add using statements to discover it.
    // ReSharper disable CheckNamespace
    public static class FilterFactory
    // ReSharper restore CheckNamespace
    {
        public static Func<string, bool> ExcludeScriptNamesInFile(string fileName)
        {
            // read script names from text file into a list,
            var scriptNames = System.IO.File.ReadAllLines(fileName).ToList();
            return (s) => { return !scriptNames.Contains(s); };
        }

        public static Func<string, bool> IncludeScriptNamesInFile(string fileName)
        {
            // read script names from text file into a list,
            var scriptNames = System.IO.File.ReadAllLines(fileName).ToList();
            return (s) => { return scriptNames.Contains(s); };
        }
    }
}


