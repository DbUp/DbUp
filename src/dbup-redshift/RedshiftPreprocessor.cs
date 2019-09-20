using DbUp.Engine;

namespace DbUp.Redshift
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with Redshift.
    /// </summary>
    public class RedshiftPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some preprocessing step on a Redshift script.
        /// </summary>
        public string Process(string contents)
        {
            return contents;
        }
    }
}
