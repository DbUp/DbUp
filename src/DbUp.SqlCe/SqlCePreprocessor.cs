using DbUp.Engine;

namespace DbUp.SqlCe
{
    /// <summary>
    /// This preprocessor makes minor adjustments to your sql to make it compatible with SqlCe
    /// </summary>
    public class SqlCePreprocessor : IScriptPreprocessor
    {
        public string Process(string contents)
        {
            return contents.Replace("nvarchar(max)", "ntext");
        }
    }
}