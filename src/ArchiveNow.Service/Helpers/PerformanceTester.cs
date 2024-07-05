using System;
using System.Diagnostics;
using ArchiveNow.Utils;

namespace ArchiveNow.Service.Helpers
{
    public class PerformanceTester : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Action<TimeSpan> _callback;

        public TimeSpan Result => _stopwatch.Elapsed;

        public PerformanceTester()
        {
            _stopwatch.Start();
        }

        public PerformanceTester(Action<TimeSpan> callback)
            : this()
        {
            _callback = callback;
        }

        public static PerformanceTester Start(Action<TimeSpan> callback)
        {
            return new PerformanceTester(callback);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _callback?.Invoke(this.Result);
        }

        public override string ToString()
        {
            return Result.ToReadableString();
        }
    }
}
