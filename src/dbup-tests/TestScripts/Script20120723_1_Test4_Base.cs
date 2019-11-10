using System;
using System.Data;
using DbUp.Engine;

namespace DbUp.Tests.TestScripts
{
    public abstract class Script20120723_1_Test4_Base : IScript
    {
        public string ProvideScript(Func<IDbCommand> commandFactory)
        {
            return ProvideScriptImplementation(commandFactory);
        }

        protected abstract string ProvideScriptImplementation(Func<IDbCommand> commandFactory);
    }
}
