using System;

namespace Infrastructure.Extensions.String
{
    public static partial class StringExtensions
    {
        public static string ToTrainCase(this string source)
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
                        return [char.ToUpperInvariant(s)];
                    }

                    return ['-', char.ToUpperInvariant(s)];
                });
        }
    }
}
