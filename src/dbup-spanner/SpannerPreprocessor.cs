using System;
using DbUp.Engine;

namespace DbUp.Spanner
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with Spanner.
    /// </summary>
    public class SpannerPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some preprocessing step on a Spanner script.
        /// </summary>
        public string Process(string contents) => contents;
    }
}
