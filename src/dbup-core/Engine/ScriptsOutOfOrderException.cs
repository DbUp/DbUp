using System;

namespace DbUp.Engine
{
    /// <inheritdoc />
    /// <summary>
    /// ScriptsOutOfOrderException thrown when attempting to run scripts in an incorrect order if EnforceScriptOrder is specified
    /// </summary>
    public class ScriptsOutOfOrderException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// ScriptsOutOfOrderException
        /// </summary>
        public ScriptsOutOfOrderException() : base("Scripts executed out of order") { }
    }
}