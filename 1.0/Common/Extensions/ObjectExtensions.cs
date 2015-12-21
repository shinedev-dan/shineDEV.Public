using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Extensions
{
    public static class ObjectExtensions
    {
        public static Int32 ParseInt32(this string s)
        {
            Int32 val = 0;
            if (!String.IsNullOrWhiteSpace(s))
                Int32.TryParse(s, out val);
            return val;
        }

        public static Int32? ParseInt32Nullable(this string s)
        {
            Int32? val = null;
            if (!String.IsNullOrWhiteSpace(s))
            {
                Int32 temp = 0;
                if (Int32.TryParse(s, out temp))
                    val = temp;
            }
            return val;
        }

        public static Int64 ParseInt64(this string s)
        {
            Int64 val = 0;
            if (!String.IsNullOrWhiteSpace(s))
                Int64.TryParse(s, out val);
            return val;
        }

        public static Int64? ParseInt64Nullable(this string s)
        {
            Int64? val = null;
            if (!String.IsNullOrWhiteSpace(s))
            {
                Int64 temp = 0;
                if (Int64.TryParse(s, out temp))
                    val = temp;
            }
            return val;
        }

        public static Boolean IsNullOrWhiteSpace(this String s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }

        public static string ToDigitsOnly(this string input)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(input, "");
        }

        public static bool In(this string val, params string[] arr)
        {
            foreach (var s in arr)
                if (val.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        public static string Cleanse(this string val, bool nullify = true)
        {
            if (val != null && val.Length > 0)
                val = val.Trim();
            if (val != null && val.Equals(string.Empty) && nullify)
                val = null;
            return val;
        }

        public static bool Matches(this string val, string compare)
        {
            if (string.IsNullOrWhiteSpace(val) && string.IsNullOrWhiteSpace(compare))
                return true;
            if (string.IsNullOrWhiteSpace(val) || string.IsNullOrWhiteSpace(compare))
                return false;
            return val.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}