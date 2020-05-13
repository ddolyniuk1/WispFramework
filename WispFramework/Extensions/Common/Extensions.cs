using System;
using System.Collections.Generic;

namespace WispFramework.Extensions.Common
{
    public static class Extensions
    {
        public static bool IsGuid(this string str)
        {
            str.ToGuid(out var success);
            return success;
        }

        public static bool IsGuidAndNotEmptyGuid(this string str)
        {
            var result = str.ToGuid(out var success);
            return success && result.IsNotDefault();
        }

        public static Guid ToGuid(this string str)
        {
            return Guid.TryParse(str, out var result) ? result : Guid.Empty;
        }

        public static bool IsDefault<T>(this T t)
        {
            return Equals(default(T), t);
        }

        public static bool IsNull<T>(this T t) where T : class
        {
            return Equals(null, t);
        }

        public static bool IsNotDefault<T>(this T t)
        {
            return !Equals(default(T), t);
        }

        public static bool IsNotNull<T>(this T t) where T : class
        {
            return !Equals(null, t);
        }

        public static Guid ToGuid(this string str, out bool success)
        {
            if (Guid.TryParse(str, out var result))
            {
                success = true;
                return result;
            }

            success = false;
            return Guid.Empty;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsNotWhitespace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static string Join(this IEnumerable<string> str, string separator)
        {
            return string.Join(separator, str);
        }

        public static string Join(this string[] str, string separator)
        {
            return string.Join(separator, str);
        }

        public static string Join(this List<string> str, string separator)
        {
            return string.Join(separator, str);
        }
    }
}
