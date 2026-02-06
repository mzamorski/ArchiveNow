using ArchiveNow.Notifications.Core;
using System;
using System.IO;
using System.Threading;

namespace ArchiveNow.Notifier
{
    internal class Program
    {
        private const string AppId = "ArchiveNow";  // AUMID
        private const string ShortcutName = "ArchiveNow Notifier";

        private static ManualResetEvent _waitHandle = new ManualResetEvent(false);

        [STAThread]
        private static void Main(string[] args)
        {
            var options = NotifierOptionsParser.Parse(args);

            INotificatioService notificatioService = new ToastNotificatioService();
            var message = new NotificationMessage()
            {
                Title = options.Title,
                Body = options.Message,
                Folder = Path.GetDirectoryName(options.Path)
            };

            notificatioService.Show(message);

            _waitHandle.WaitOne(TimeSpan.FromSeconds(10));
        }
    }
}