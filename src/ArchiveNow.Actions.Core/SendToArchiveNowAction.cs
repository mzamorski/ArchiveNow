using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using ArchiveNow.Actions.Core.Contexts;
using ArchiveNow.Actions.Core.Result;

namespace ArchiveNow.Actions.Core
{
    /// <summary>
    /// Sends the produced archive file to another machine that runs ArchiveNow and
    /// exposes a simple HTTP endpoint. The remote instance is expected to listen on
    /// a given host and port and accept a POST request at '/upload' containing the
    /// file stream.
    /// </summary>
    public class SendToArchiveNowAction : AfterFinishedActionBase
    {
        private readonly string _host;
        private readonly int _port;

        public override string Description => "Sending to remote instance...";

        public SendToArchiveNowAction(SendToArchiveNowContext context)
            : base(precedence: 7)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _host = context.Host;
            _port = context.Port;
        }

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var hasError = false;
            var message = string.Empty;
            var fileName = Path.GetFileName(context.InputPath);

            try
            {
                var target = new UriBuilder("http", _host, _port, "/upload").Uri;
                using (var client = new HttpClient())
                using (var stream = File.OpenRead(context.InputPath))
                using (var content = new StreamContent(stream))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Headers.Add("X-FileName", fileName);

                    var response = client.PostAsync(target, content).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        hasError = true;
                        message = response.ReasonPhrase;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                hasError = true;
                message = ex.Message;
            }

            return new AfterFinishedActionResult(!hasError, context.InputPath, message);
        }
    }
}
