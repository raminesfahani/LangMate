using System;

namespace Infrastructure.Extensions.String
{
    public static partial class StringExtensions
    {
        public static string ToPascalCase(this string source)
        {
            return source == null
                ? throw new ArgumentNullException(nameof(source))
                : SymbolsPipe(
                source,
                '\0',
                (s, i) => new char[] { char.ToUpperInvariant(s) });
        }
    }
}
