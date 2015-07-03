using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Haven
{
    public class Password
    {
        public static string HashPassword(string password)
        {
            // from http://stackoverflow.com/questions/4181198/how-to-hash-a-password
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string passwordToVerify)
        {
            if (password == null || passwordToVerify == null)
            {
                return false;
            }

            // from http://stackoverflow.com/questions/4181198/how-to-hash-a-password
            byte[] hashBytes = Convert.FromBase64String(password);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(passwordToVerify, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);
            return !(hash.Where((x, i) => x != hashBytes[i + 16]).Any());
        }
    }
}
