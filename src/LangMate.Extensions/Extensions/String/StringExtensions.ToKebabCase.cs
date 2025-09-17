using System;

namespace Infrastructure.Extensions.String
{
    public static partial class StringExtensions
    {
        public static string ToKebabCase(this string source)
        {
            return source == null
                ? throw new ArgumentNullException(nameof(source))
                : SymbolsPipe(
                source,
                '-',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return [char.ToLowerInvariant(s)];
                    }

                    return ['-', char.ToLowerInvariant(s)];
                });
        }
    }
}
