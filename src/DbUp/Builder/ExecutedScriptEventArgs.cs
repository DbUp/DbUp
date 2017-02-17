using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbUp.Engine;

namespace DbUp.Builder
{
    public class ExecutedScriptEventArgs : EventArgs
    {
        private string _scriptName;
        private string _scriptContents;

        internal ExecutedScriptEventArgs(SqlScript script)
        {
            _scriptName = script.Name;
            _scriptContents = script.Contents;
        }

        /// <summary>
        /// Returns the name of the executed script.
        /// </summary>
        public string ScriptName
        {
            get { return _scriptName; }
        }

        /// <summary>
        /// Returns the contents of the executed script.
        /// </summary>
        public string ScriptContents
        {
            get { return _scriptContents; }
        }
    }
}
