using System;
using System.Security.Cryptography;
using System.Text;

namespace Threesixty.Dal.Bll
{
    public class CryptoUtils
    {
        // some random bytes 
        private static readonly byte[] SaltBytes = { 125, 83, 184, 8 };

        public static string CalculateHash(string data)
        {
            string hash;
            using (var hashAlgorithm = SHA512.Create())
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var saltedBytes = new byte[SaltBytes.Length + dataBytes.Length];
                Buffer.BlockCopy(SaltBytes, 0, saltedBytes, 0, SaltBytes.Length);
                Buffer.BlockCopy(dataBytes, 0, saltedBytes, SaltBytes.Length, dataBytes.Length);
                var hashedBytes = hashAlgorithm.ComputeHash(saltedBytes);
                hash = Convert.ToBase64String(hashedBytes);
            }

            return hash;
        }
    }
}
