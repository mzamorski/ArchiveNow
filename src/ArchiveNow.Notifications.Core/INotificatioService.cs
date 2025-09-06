using System;

namespace ArchiveNow.Notifications.Core
{
    public interface INotificatioService
    {
        void Show(NotificationMessage message);
    }
}