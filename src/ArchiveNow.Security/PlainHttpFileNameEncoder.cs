using System.Net;

namespace ArchiveNow.Security
{
    public sealed class PlainHttpFileNameEncoder : IHttpFileNameEncoder
    {
        public string? Decode(HttpListenerRequest req)
        {
            return req.Headers[ArchiveNowHttpHeaders.FileName];
        }

        public void Encode(HttpClient client, HttpContent content, string fileName)
        {
            content.Headers.Add(ArchiveNowHttpHeaders.FileName, fileName);
        }
    }
}