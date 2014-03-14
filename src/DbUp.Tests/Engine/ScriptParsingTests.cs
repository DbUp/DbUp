using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using NUnit.Framework;

namespace DbUp.Tests.Engine
{
    public class ScriptParsingTests
    {
        [TestFixture]
        public class When_Parsing_A_Script_With_GO_separators : SpecificationFor<SqlBatchParser>
        {
            private IEnumerable<string> batches;

            public override SqlBatchParser Given()
            {
                return new SqlBatchParser();
            }

            public override void When()
            {
                var sql = @"SELECT 1
GO
SELECT 2";
                batches = Subject.SplitScriptBatches(sql, "GO");
            }

            [Then]
            public void it_returns_the_right_number_of_batches()
            {
                Assert.AreEqual(2, batches.Count());
            }

            [Then]
            public void it_returns_the_right_content()
            {
                Assert.AreEqual(new String[]{"SELECT 1", "SELECT 2"}, batches);
            }


        }
 
    }
}