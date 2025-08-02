using System;
using System.IO;
using System.Threading;
using ArchiveNow.Actions.Core;

namespace ArchiveNow.Service
{
    /// <summary>
    /// Wraps <see cref="RemoteUploadListener"/> to run within the service layer.
    /// </summary>
    public class RemoteUploadService : IDisposable
    {
        private readonly RemoteUploadListener _listener;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public RemoteUploadService(int port, string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentException("Output directory must be provided", nameof(outputDirectory));
            }

            Directory.CreateDirectory(outputDirectory);
            _listener = new RemoteUploadListener(port, outputDirectory);
        }

        public void Start()
        {
            _listener.Start(_cts.Token);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _listener.Dispose();
        }
    }
}

