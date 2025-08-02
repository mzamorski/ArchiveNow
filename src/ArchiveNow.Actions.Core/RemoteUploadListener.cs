using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveNow.Actions.Core
{
    /// <summary>
    /// Simple HTTP listener that accepts archives sent by SendToArchiveNowAction.
    /// It listens for POST requests at '/upload' and stores the body as a file
    /// using the value from the 'X-FileName' header.
    /// </summary>
    public class RemoteUploadListener : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly string _outputDirectory;

        public RemoteUploadListener(int port, string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory must be provided", nameof(outputDirectory));

            _outputDirectory = outputDirectory;
            Directory.CreateDirectory(_outputDirectory);

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
        }

        public void Start(CancellationToken token)
        {
            _listener.Start();
            Task.Run(() => ListenLoop(token), token);
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch (HttpListenerException)
                {
                    break;
                }

                if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/upload")
                {
                    var fileName = context.Request.Headers["X-FileName"] ?? $"upload_{DateTime.UtcNow.Ticks}";
                    var destinationPath = Path.Combine(_outputDirectory, fileName);

                    using (var file = File.Create(destinationPath))
                    {
                        await context.Request.InputStream.CopyToAsync(file);
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                context.Response.Close();
            }
        }

        public void Dispose()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
            _listener.Close();
        }
    }
}
