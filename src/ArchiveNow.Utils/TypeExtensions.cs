using System;

namespace ArchiveNow.Utils
{
    public static class TypeExtensions
    {
        public static bool HasValue(this string value)
        {
            if (value == default(string))
            {
                return false;
            }

            return !string.IsNullOrEmpty(value);
        }

        public static bool HasValue(this string value, Func<string, bool> checker)
        {
            return checker(value);
        }

        public static bool HasValue<T>(this T value)
            where T : class
        {
            return (value == default(T));
        }

        public static bool HasValue<T>(this T? value) 
            where T : struct
        {
            return !value.HasValue;
        }
    }
}
