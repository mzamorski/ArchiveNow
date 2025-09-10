using Microsoft.Toolkit.Uwp.Notifications;

namespace ArchiveNow.Notifications.Core
{
    public sealed class ToastNotificatioService : INotificatioService
    {
        public ToastNotificatioService()
        { }

        public void Show(NotificationMessage msg)
        {
            var builder = new ToastContentBuilder()
                //.AddAppLogoOverride(new Uri(iconFilePath))
                .AddText(msg.Title)
                .AddText(msg.Body)
                .SetToastScenario(ToastScenario.Alarm);

            if (!string.IsNullOrEmpty(msg.LaunchArgs))
            {
                builder.AddArgument("launch", msg.LaunchArgs);
            }

            //builder.Show(toast => {
            //    toast.Tag = "AcXgMobile";
            //});
        }
    }
}