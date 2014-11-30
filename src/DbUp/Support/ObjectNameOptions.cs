using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.Support
{
    /// <summary>
    /// Object name parser options
    /// </summary>
    public enum ObjectNameOptions
    {
        /// <summary>
        /// No options are set.
        /// </summary>
        None,

        /// <summary>
        /// Remove starting and ending white space from the object name.
        /// </summary>
        Trim
    }
}
