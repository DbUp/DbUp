using System;
using System.Security.Cryptography;
using System.Text;

namespace DbUp.Helpers
{
    /// <summary>
    /// String hasher
    /// </summary> 
    public class Hasher : IHasher
    {
        /// <summary>
        /// Returns hash of input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Hashed input</returns> 
        public string GetHash(
            string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] checksum = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
            }
        }
    }
}
