using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Threesixty.Dal.Bll
{
    public class CryptoUtils
    {
        private static string _phrase = "what is love? baby Dont Hurt M3!";

        // some random bytes 
        public static readonly byte[] SaltBytes = Encoding.UTF8.GetBytes(_phrase);

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
