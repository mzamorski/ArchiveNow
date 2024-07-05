using System;

namespace ArchiveNow.Core.Loggers
{
    public class EmptyArchiveNowLogger : IArchiveNowLogger
    {
        private static readonly Lazy<EmptyArchiveNowLogger> _instance = new Lazy<EmptyArchiveNowLogger>(() => new EmptyArchiveNowLogger());

        public static EmptyArchiveNowLogger Instance => _instance.Value;

        private EmptyArchiveNowLogger()
        { }

        public void Info(string message)
        { }

        public void Error(string message)
        { }

        public void Debug(string message)
        { }

        public void Warning(string message)
        { }
    }
}
