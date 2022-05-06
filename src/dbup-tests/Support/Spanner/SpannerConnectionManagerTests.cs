using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Spanner;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.Spanner
{
    public class SpannerConnectionManagerTests
    {
        [Fact]
        public void CanParseSingleLineScript()
        {
            const string singleCommand = "CREATE TABLE Test (id INT64 NOT NULL, name STRING(50)) PRIMARY KEY(name);";

            var connectionManager = new SpannerConnectionManager("connectionstring");

            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void CanParseMultilineScript()
        {
            var multiCommand = "CREATE TABLE Test (id INT64 NOT NULL, name STRING(50)) PRIMARY KEY(name);";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE Singers (id INT64 NOT NULL, name STRING(50)) PRIMARY KEY(name);";

            var connectionManager = new SpannerConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(2);
        }
    }
}
