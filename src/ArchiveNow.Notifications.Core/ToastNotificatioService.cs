using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace ArchiveNow.Notifications.Core
{
    public sealed class ToastNotificatioService : INotificatioService
    {
        public ToastNotificatioService()
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                // Obtain any user input (text boxes, menu selections) from the notification
                ValueSet userInput = toastArgs.UserInput;

            };
        }

        public void Show(NotificationMessage msg)
        {
            //var iconFilePath = Path.Combine(AppContext.BaseDirectory, "an-main.ico");

            var builder = new ToastContentBuilder()
                //.AddAppLogoOverride(new Uri(iconFilePath))
                .AddText(msg.Title)
                .AddText(msg.Body);

            if (!string.IsNullOrEmpty(msg.LaunchArgs))
                builder.AddArgument("launch", msg.LaunchArgs);

            var notif = new ToastNotification(builder.GetToastContent().GetXml());

            if (!string.IsNullOrEmpty(msg.Tag)) notif.Tag = msg.Tag;
            if (!string.IsNullOrEmpty(msg.Group)) notif.Group = msg.Group;

            ToastNotificationManagerCompat
                .CreateToastNotifier()
                .Show(notif);
        }
    }
}