using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.IO;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace ArchiveNow.Notifications.Core
{
    public sealed class ToastNotificatioService : INotificatioService
    {
        public ToastNotificatioService()
        {
            ToastNotificationManagerCompat.OnActivated += args =>
            {
                ToastArguments toastArgs = ToastArguments.Parse(args.Argument);

                if (toastArgs.TryGetValue("Folder", out string folderPath))
                {
                    if (Directory.Exists(folderPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = folderPath,
                            UseShellExecute = true
                        });
                    }
                }
            };
        }

        public void Show(NotificationMessage msg)
        {
            if (!AreNotificationsEnabledForApp(out var reason))
            {
                // TODO
                //
                // Windows Services cannot display UI elements such as MessageBox because they run in session 0
                // without access to the interactive user desktop. Any attempt to show a dialog from a service
                // will either fail silently or block the service. User-facing notifications must be triggered
                // from the WPF application, not from the service process.

                //MessageBox.Show(reason + " Please enable notifications in Windows settings.", "ArchiveNow", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                return;
            }

            var builder = new ToastContentBuilder()
                //.AddAppLogoOverride(new Uri(iconFilePath))
                .AddText(msg.Title)
                .AddText(msg.Body)
                .SetToastScenario(ToastScenario.Alarm);

            if (!string.IsNullOrEmpty(msg.Folder))
            {
                builder.AddArgument("Folder", msg.Folder);
            }

            var notif = new ToastNotification(builder.GetToastContent().GetXml());
    
            if (!string.IsNullOrEmpty(msg.Tag)) notif.Tag = msg.Tag;
            if (!string.IsNullOrEmpty(msg.Group)) notif.Group = msg.Group;

            ToastNotificationManagerCompat
                .CreateToastNotifier()
                .Show(notif);
        }

        private bool AreNotificationsEnabledForApp(out string reason)
        {
            var setting = ToastNotificationManagerCompat.CreateToastNotifier().Setting;

            switch (setting)
            {
                case NotificationSetting.Enabled:
                    reason = null;
                    return true;

                case NotificationSetting.DisabledForApplication:
                    reason = "Notifications are disabled for this application in Windows settings.";
                    return false;

                case NotificationSetting.DisabledForUser:
                    reason = "Notifications are disabled for the current user in Windows settings.";
                    return false;

                case NotificationSetting.DisabledByGroupPolicy:
                    reason = "Notifications are disabled by system policy.";
                    return false;

                case NotificationSetting.DisabledByManifest:
                    reason = "Notifications are disabled by app manifest.";
                    return false;

                default:
                    reason = "Notifications are not available.";
                    return false;
            }
        }
    }
}