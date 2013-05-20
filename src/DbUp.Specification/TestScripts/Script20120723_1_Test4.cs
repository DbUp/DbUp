using System;
using System.Data;
using DbUp.Engine;

namespace DbUp.Specification.TestScripts
{
    public class Script20120723_1_Test4 : IScript
    {
        public string ProvideScript(Func<IDbCommand> commandFactory)
        {
            return "test4";
        }
    }
}
