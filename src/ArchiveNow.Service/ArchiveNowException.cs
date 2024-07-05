using System;

namespace ArchiveNow.Service
{
    public class ArchiveNowException : ApplicationException
    {
        public ArchiveNowException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}