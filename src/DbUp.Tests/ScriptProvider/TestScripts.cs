using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ApprovalUtilities.Utilities;

namespace DbUp.Tests.ScriptProvider
{
    static class TestScripts
    {
        public static void Create(out string testPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            testPath = CreateTestPathBasedOnAssemblyLocation(assembly);

            foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
            {
                using (var stream = assembly.GetManifestResourceStream(scriptName))
                {
                    var filePath = Path.Combine(testPath, GetScriptPathAndName(scriptName));
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var writer = new FileStream(filePath, FileMode.Create))
                    {
                        stream.CopyTo(writer);
                        writer.Flush();
                        writer.Close();
                    }
                    stream.Close();
                }
            }
        }

        private static string GetScriptPathAndName(string scriptName)
        {
            var dir = Regex.Match(scriptName, @"\.(Folder\d)");
            if (dir.Success)
            {
                return Path.Combine(dir.Groups[1].Value, scriptName.Replace(dir.Value, ""));
            }
            return scriptName;
        }

        private static string CreateTestPathBasedOnAssemblyLocation(Assembly assembly)
        {
            var directory = new FileInfo(assembly.Location).DirectoryName;
            var testPath = Path.Combine(directory, "sqlfiles");
            Directory.CreateDirectory(testPath);
            return testPath;
        }
    }
}
