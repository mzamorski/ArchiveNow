using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveNow.Utils
{
    public static class TimeSpanExtensions
    {
        public static string ToReadableString(this TimeSpan span)
        {
            return string.Join(", ", span.GetReadableStringElements()
                .Where(str => !string.IsNullOrWhiteSpace(str)));
        }

        private static IEnumerable<string> GetReadableStringElements(this TimeSpan span)
        {
            yield return GetDaysString((int)Math.Floor(span.TotalDays));
            yield return GetHoursString(span.Hours);
            yield return GetMinutesString(span.Minutes);
            yield return GetSecondsString(span.Seconds);
        }

        private static string GetDaysString(int days)
        {
            switch (days)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "1 day";
            }

            return $"{days:0} days";
        }

        private static string GetHoursString(int hours)
        {
            switch (hours)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "1 hour";
            }

            return $"{hours:0} hours";
        }

        private static string GetMinutesString(int minutes)
        {
            switch (minutes)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "1 minute";
            }

            return $"{minutes:0} minutes";
        }

        private static string GetSecondsString(int seconds)
        {
            switch (seconds)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "1 second";
            }

            return $"{seconds:0} seconds";
        }
    }
}
