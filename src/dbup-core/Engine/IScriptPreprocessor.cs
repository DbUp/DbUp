namespace DbUp.Engine
{
    /// <summary>
    /// Preprocessor that executes before scripts are executed against the database
    /// </summary>
    public interface IScriptPreprocessor
    {
        /// <summary>
        /// Performs some preprocessing step on a script
        /// </summary>
        string Process(string contents);
    }
}