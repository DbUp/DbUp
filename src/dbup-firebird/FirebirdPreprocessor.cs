﻿using DbUp.Engine;
using System;

namespace DbUp.Firebird
{
    /// <summary>
    /// This preprocessor makes adjustments to your sql to make it compatible with Firebird.
    /// </summary>
    public class FirebirdPreprocessor : IScriptPreprocessor
    {
        /// <summary>
        /// Performs some proprocessing step on a Firebird script.
        /// </summary>
        public string Process(string contents)
        {
            return contents;
        }
    }
}
