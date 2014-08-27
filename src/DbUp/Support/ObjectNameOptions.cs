using System;

namespace DbUp.Support
{
    /// <summary>
    /// Object Name options
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
