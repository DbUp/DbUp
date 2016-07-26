using System;

namespace DbUp.Engine.Preprocessors
{
    /// <summary>
    /// Used for database engines that do not need any preprocessor
    /// </summary>
    public class NullPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs nothing
        /// </summary>
        public string Process(string contents)
        {
            return contents;
        }
    }
}
