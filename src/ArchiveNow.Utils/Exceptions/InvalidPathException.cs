using System;

namespace ArchiveNow.Utils.Exceptions
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }

        public InvalidPathException()
            : this("The specified path is invalid.")
        {
        }
    }
}