using System;

namespace ArchiveNow.Utils.Exceptions
{
    public class PathNotFoundException : Exception
    {
        public PathNotFoundException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }

        public PathNotFoundException()
            : this("The specified path is not found.")
        {
        }
    }
}
