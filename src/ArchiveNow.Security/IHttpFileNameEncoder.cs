using System.Net;

namespace ArchiveNow.Security
{
    public interface IHttpFileNameEncoder
    {
        /// <summary>
        /// Extracts and decodes the filename from an HTTP request.
        /// Always returns a sanitized filename suitable for saving on disk.
        /// </summary>
        string? Decode(HttpListenerRequest req);

        /// <summary>
        /// Encodes the filename into headers for an outgoing HTTP request.
        /// </summary>
        void Encode(HttpClient client, HttpContent content, string fileName);
    }
}
