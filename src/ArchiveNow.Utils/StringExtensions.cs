using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace ArchiveNow.Utils
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static bool IsNotEmpty(this string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static string ReplaceLast(this string input, string oldValue, string newValue)
        {
            int place = input.LastIndexOf(oldValue, StringComparison.Ordinal);

            return input.Remove(place, 1).Insert(place, newValue);
        }

        public static string Concat(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }

        public static SecureString ConvertToSecureString(this string source)
        {
            var result = new SecureString();

            if (string.IsNullOrWhiteSpace(source))
            {
                return result;
            }

            foreach (char c in source)
            {
                result.AppendChar(c);
            }

            return result;
        }

        public static string ConvertToString(this SecureString value)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);

                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
