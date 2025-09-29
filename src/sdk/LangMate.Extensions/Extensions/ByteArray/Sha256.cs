using System;
using System.Security.Cryptography;
using System.Text;

namespace LangMate.Extensions.Extensions.ByteArray
{
    public static class Sha256
    {

        /// <summary>
        /// Calculate sha256 of a byte arrayy value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSha256(this byte[] value)
        {

            StringBuilder Sb = new();
            byte[] result = SHA256.HashData(value);

            foreach (byte b in result)
                Sb.Append(b.ToString("x2"));

            return Sb.ToString();
        }
    }
}
