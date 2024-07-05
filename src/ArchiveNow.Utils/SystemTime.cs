using System;

namespace ArchiveNow.Utils
{
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;

        public static void Set(DateTime date)
        {
            Now = () => date;
        }

        public static void Reset()
        {
            Now = () => DateTime.Now;
        }
    }
}
