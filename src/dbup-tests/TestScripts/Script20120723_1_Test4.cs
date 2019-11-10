using System;
using System.Data;

namespace DbUp.Tests.TestScripts
{
    public class Script20120723_1_Test4 : Script20120723_1_Test4_Base
    {
        protected override string ProvideScriptImplementation(Func<IDbCommand> commandFactory)
        {
            return "test4";
        }
    }
}
