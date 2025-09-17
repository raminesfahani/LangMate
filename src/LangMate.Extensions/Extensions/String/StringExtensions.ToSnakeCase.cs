using System;

namespace Infrastructure.Extensions.String
{
    public static partial class StringExtensions
    {
        public static string ToSnakeCase(this string source)
        {
            return source == null
                ? throw new ArgumentNullException(nameof(source))
                : SymbolsPipe(
                source,
                '_',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return [char.ToLowerInvariant(s)];
                    }

                    return ['_', char.ToLowerInvariant(s)];
                });
        }
    }
}
