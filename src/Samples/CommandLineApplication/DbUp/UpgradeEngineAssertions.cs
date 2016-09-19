using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DbUp.Engine;
using DbUp.ScriptProviders;
using System.Linq;

namespace CommandLineApplication.DbUp
{
    public static class UpgradeEngineAssertions
    {

        public static void AssertThatAnyNewScriptsHaveNotAlreadyBeenExecuted(this UpgradeEngine upgrader, string scriptNamespace)
        {
            AssertThatAnyNewScriptsHaveNotAlreadyBeenExecuted(upgrader, x => x.EndsWith(".sql") && x.StartsWith(scriptNamespace));
        }

        public static void AssertThatAnyNewScriptsHaveNotAlreadyBeenExecuted(this UpgradeEngine upgrader, Func<string, bool> filter)
        {
            var allScripts = new EmbeddedScriptsProvider(new[] {typeof(Program).Assembly}, filter, Encoding.Default)
                .GetScripts(null).ToDictionary(script => script.Name, new IgnoreStableWhenComparingScriptNames());

            var executedScripts = upgrader.GetExecutedScripts().Select(x => allScripts[x]).ToArray();
            var newScripts = upgrader.GetScriptsToExecute().ToArray();

            foreach (var executedScript in executedScripts)
            {
                var executedScriptHash = CreateHashForScript(executedScript);
                foreach (var newScript in newScripts)
                {
                    var newScriptHash = CreateHashForScript(newScript);
                    if (executedScriptHash.SequenceEqual(newScriptHash))
                    {
                        throw new Exception($"{newScript.Name} with MD5: {newScriptHash} has already been executed " +
                            $"in script {executedScript.Name} with MD5: {executedScriptHash}");
                    }
                }
            }
        }

        private static string CreateHashForScript(SqlScript script)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = new MemoryStream(Encoding.Default.GetBytes(script.Contents)))
                {
                    var hash = md5.ComputeHash(stream);
                    var result = new StringBuilder();
                    foreach (var t in hash)
                        result.Append(t.ToString("X2"));

                    return result.ToString();
                }
            }
        }

        private class IgnoreStableWhenComparingScriptNames : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Replace(".Stable", string.Empty) == y.Replace(".Stable", string.Empty);
            }

            public int GetHashCode(string s)
            {
                return s.Replace(".Stable", string.Empty).GetHashCode();
            }
        }
    }
}
