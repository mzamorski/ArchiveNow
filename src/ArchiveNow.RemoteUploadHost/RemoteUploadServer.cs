using System.Net;

namespace ArchiveNow.RemoteUpload.Server
{
    public class RemoteUploadServer : IDisposable
    {
        private readonly RemoteUploadConfiguration _config;
        private readonly HttpListener _listener;
        private CancellationTokenSource _cts;

        public RemoteUploadServer(RemoteUploadConfiguration config)
        {
            _config = config;

            var uploadsDirectory = _config.UploadsDirectory;

            if (string.IsNullOrWhiteSpace(uploadsDirectory))
                throw new ArgumentException("Uploads directory must be provided", nameof(uploadsDirectory));

            Directory.CreateDirectory(uploadsDirectory);

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{_config.Port}/");
        }

        public void Start()
        {
            if (_cts != null)
            {
                throw new InvalidOperationException("Host already started");
            }

            _cts = new CancellationTokenSource();
            _listener.Start();
            Task.Run(() => ListenLoop(_cts.Token));
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    Console.WriteLine("Waiting for connection...");
                    context = await _listener.GetContextAsync();
                }
                catch (HttpListenerException) when (token.IsCancellationRequested)
                {
                    break;
                }

                _ = Task.Run(() => HandleContextAsync(context), token);
            }
        }

        private async Task HandleContextAsync(HttpListenerContext context)
        {
            Console.WriteLine($"Connection handle: {context.Request}");

            if (context.Request.HttpMethod != "POST")
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            var fileName = context.Request.Headers["X-FileName"] ?? $"upload_{DateTime.UtcNow.Ticks}";
            var filePath = Path.Combine(_config.UploadsDirectory, fileName);

            Console.WriteLine($"Sending {fileName}...");

            using (var fs = File.Create(filePath))
            {
                await context.Request.InputStream.CopyToAsync(fs);
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Close();

            Console.WriteLine("Done.");
        }

        public void Stop()
        {
            if (_cts == null)
            {
                return;
            }

            _cts.Cancel();
            _listener.Stop();
            _cts.Dispose();
        }

        public void Dispose()
        {
            Stop();
            _listener.Close();
        }
    }
}

