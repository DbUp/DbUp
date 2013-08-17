using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using NSubstitute;

namespace DbUp.Tests.Specifications.Contexts
{
    public abstract class GivenScriptsWithVariableSubstitutions : EmptyDatabase
    {
        public override void BeforeEach()
        {
            base.BeforeEach();

            AllScripts = new List<SqlScript>
                             {
                                 new SqlScript("0001.sql", "CREATE TABLE $sometable$ (Id int)")
                             };

            ScriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(AllScripts);
            VersionTracker.GetExecutedScripts().Returns(new string[0]);
        }
    }
}