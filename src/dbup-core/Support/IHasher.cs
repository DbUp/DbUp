using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DbUp.Support
{
    public interface IHasher
    {
        string GetHash(string input);
    }

    public class Hasher : IHasher
    {
        public string GetHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] checksum = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
