using System;
using System.Security.Cryptography;
using System.Text;

namespace LangMate.Extensions.Extensions.String
{
    public static class Sha256
    {
        /// <summary>
        /// Calculate sha256 of a string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSha256(this string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
