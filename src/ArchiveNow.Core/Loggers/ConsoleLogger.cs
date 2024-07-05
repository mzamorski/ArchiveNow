using System;

namespace ArchiveNow.Core.Loggers
{
    public class ConsoleLogger : IArchiveNowLogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Warning(string message)
        {
            Console.WriteLine(message);
        }
    }
}
