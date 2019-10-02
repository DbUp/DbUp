using DbUp.Engine;

namespace DbUp.Oracle
{
    public class OraclePreprocessor : IScriptPreprocessor
    {
        public string Process(string contents) => contents;
    }
}
