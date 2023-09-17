using System;
using System.Text;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    public class EmbeddedScriptsOptions
    {
        public Func<string, bool> Filter { get; set; }
        public Func<string, string> ScriptNameProvider { get; set; }
        public Encoding Encoding { get; set; }
        public SqlScriptOptions SqlScriptOptions { get; set; }
    }

}
