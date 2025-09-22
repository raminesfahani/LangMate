using System.Security.Cryptography;
using System.Text;

namespace LangMate.Persistence.Cache
{
    public static class CacheKeyHelper
    {
        public static string GenerateKey(string model, string language, string prompt)
        {
            var input = $"{model}|{language}|{prompt}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash);
        }
    }
}
