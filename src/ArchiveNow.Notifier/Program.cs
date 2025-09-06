using ArchiveNow.Notifications.Core;
using System;

namespace ArchiveNow.Notifier
{
    internal class Program
    {
        private const string AppId = "ArchiveNow.Notifier";  // AUMID
        private const string ShortcutName = "ArchiveNow Notifier";

        [STAThread]
        private static void Main(string[] args)
        {
            var options = NotifierOptionsParser.Parse(args);

            INotificatioService notificatioService = new ToastNotificatioService();
            var message = new NotificationMessage()
            {
                Title = options.Title,
                Body = options.Message
            };

            notificatioService.Show(message);
        }
    }
}