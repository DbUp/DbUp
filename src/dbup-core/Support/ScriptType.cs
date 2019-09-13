namespace DbUp.Support
{
    public enum ScriptType
    {
        /// <summary>
        /// Default script type.  The script will run only once.
        /// </summary>
        RunOnce = 0,

        /// <summary>
        /// The script will always be run.  Useful for setting permissions.  Please note, the script should be written so it can always be ran.
        /// </summary>
        RunAlways = 1
    }
}
