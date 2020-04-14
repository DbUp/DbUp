namespace DbUp.Helpers
{
    /// <summary>
    /// String hasher interface
    /// </summary> 
    public interface IHasher
    {
        /// <summary>
        /// Returns hash of input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Hashed input</returns> 
        string GetHash(string input);
    }
}
