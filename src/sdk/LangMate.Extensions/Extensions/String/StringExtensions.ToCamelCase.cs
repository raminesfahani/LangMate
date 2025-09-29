using System;

namespace Infrastructure.Extensions.String
{
    public static partial class StringExtensions
    {
        public static string ToCamelCase(this string source)
        {
            return source == null
                ? throw new ArgumentNullException(nameof(source))
                : SymbolsPipe(
                source,
                '\0',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return [char.ToLowerInvariant(s)];
                    }

                    return [char.ToUpperInvariant(s)];
                });
        }
    }
}
