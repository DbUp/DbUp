using System.Collections.Generic;
using DbUp.Engine;
using DbUp.ScriptProviders;
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

            ScriptProvider.GetScripts().Returns(AllScripts);
            VersionTracker.GetExecutedScripts().Returns(new string[0]);
        }
    }
}