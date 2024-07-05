using System;

namespace ArchiveNow.Core
{
    public class ArchiveNowException : ApplicationException
    {
        public ArchiveNowException(string message, Exception innerException = null)
            : base(message, innerException)
        { }
    }
}