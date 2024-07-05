using System;

namespace ArchiveNow.Utils
{
    public static class PercentExtensions
    {
        public static decimal PercentOf(this double percent, double from)
        {
            return Convert.ToDecimal(percent * from / 100d);
        }

        public static decimal AsPercentOf(this double value, double from)
        {
            return Convert.ToDecimal(value / from * 100d);
        }

        public static decimal AsPercentOf(this long value, long from)
        {
            return AsPercentOf(Convert.ToDouble(value), Convert.ToDouble(from));
        }

        public static decimal AsPercentOf(this int value, int from)
        {
            return AsPercentOf(Convert.ToDouble(value), Convert.ToDouble(from));
        }
    }
}