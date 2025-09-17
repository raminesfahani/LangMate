using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LangMate.Extensions.Extensions.String
{
    /// <summary>
    /// Convert cases as per example: https://en.wikipedia.org/wiki/Naming_convention_(programming)#Examples_of_multiple-word_identifier_formats
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        /// Convert "Normal string" into "snake_case" string.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string StringToSnakeCase(this string original)
        {
            string result = original.Replace(" ", "_").ToLower();
            // remove trailing dot
            result = Regex.Replace(result, @"\.$", "");
            return result;
        }

        /// <summary>
        /// Convert PascalCase string into snake_case string
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string PascalToSnakeCase(this string original)
        {
            string result = string.Concat(original.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            // remove trailing dot
            result = Regex.Replace(result, @"\.$", "");
            return result;
        }

        public static string ToPascalCase(this string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            IEnumerable<string> pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static string AddSpaceToPascalCase(this string original)
        {
            string result = string.Concat(original.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));
            return result;
        }

        /// <summary>
        /// Convert any string into CONSTANT_BASE_FROMAT to be used as a constant name of a variable.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="addPrefixUnderline">In case name starts with a digit, it will add _ as prefix.</param>
        /// <returns></returns>
        public static string ToConstantCase(this string original, bool addPrefix = false)
        {


            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            IEnumerable<string> constantCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => w.ToUpper());


            string joined = string.Join("_", constantCase);
            if (addPrefix && Regex.IsMatch(joined, @"^\d"))
                joined = "_" + joined;

            return joined;
        }

        public static string EscapeQuotes(this string original)
        {
            return original.Replace(@"""", @"""""");
        }

        public static string ToCodeString(this Uri original)
        {
            if (original is null)
                return "null";

            return string.IsNullOrEmpty(original.ToString()) ? "null" : $@"new Uri(""{original}"")";
        }
    }
}
