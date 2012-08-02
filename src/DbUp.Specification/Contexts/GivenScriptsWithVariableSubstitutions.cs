using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using NSubstitute;

namespace DbUp.Specification.Contexts
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

            ScriptProvider.GetScripts(Arg.Any<Func<IDbConnection>>()).Returns(AllScripts);
            VersionTracker.GetExecutedScripts().Returns(new string[0]);
        }
    }
}