using LangMate.Extensions.Extensions.String;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace LangMate.Extensions.Extensions.Object
{
    public static class QueryString
    {
        /// <summary>
        /// Generate a Web QueryString using reflection from any object with public properties that has
        /// value other than null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetQueryString(this object obj, SelectCase @case = SelectCase.NoChange)
        {
            IEnumerable<string> properties;
            switch (@case)
            {
                case SelectCase.NoChange:
                    properties = obj.GetType().GetRuntimeProperties()
                                    .Where(p => p.GetValue(obj, null) != null)
                                    .Select(p => p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null).ToString()));
                    break;

                case SelectCase.PascalToSnakeCase:
                    properties = obj.GetType().GetRuntimeProperties()
                                    .Where(p => p.GetValue(obj, null) != null)
                                    .Select(p => p.Name.PascalToSnakeCase() + "=" + WebUtility.UrlEncode(p.GetValue(obj, null).ToString()));
                    break;

                default:
                    throw new KeyNotFoundException();
            }

            return string.Join("&", properties.ToArray());
        }
    }


    public enum SelectCase
    {
        NoChange,
        PascalToSnakeCase
    }
}