using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace REST_API.Utilities
{
    public static class HashUtility
    {
        private static int saltSize = 32;
        private static int hashSize = 20;
        private static int iterations = 10000;

        private static int tokenSize = 64;

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] hash;

            using (Rfc2898DeriveBytes alg = new Rfc2898DeriveBytes(password, saltSize, iterations))
            {
                hash = alg.GetBytes(hashSize);
                salt = alg.Salt;
            }

            byte[] result = new byte[saltSize + hashSize];

            Array.Copy(salt, 0, result, 0, saltSize);
            Array.Copy(hash, 0, result, saltSize, hashSize);

            return Convert.ToBase64String(result);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);

            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            byte[] newHash;

            using (Rfc2898DeriveBytes alg = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                newHash = alg.GetBytes(hashSize);
            }

            for (int i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + saltSize] != newHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static string GenerateNewToken()
        {
            byte[] res = new byte[tokenSize];

            using (RNGCryptoServiceProvider rand =  new RNGCryptoServiceProvider())
            {
                rand.GetBytes(res);
            }

            return Convert.ToBase64String(res);
        }

    }
}