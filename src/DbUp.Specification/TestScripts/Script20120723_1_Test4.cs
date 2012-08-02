using System;
using System.Data;
using DbUp.Engine;

namespace DbUp.Specification.TestScripts
{
    public class Script20120723_1_Test4 : IScript
    {
        public string ProvideScript(IDbConnection sqlConnectionString)
        {
            return "test4";
        }
    }
}
