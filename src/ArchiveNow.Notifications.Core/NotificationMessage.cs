namespace ArchiveNow.Notifications.Core
{
    public sealed class NotificationMessage
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationLevel Level { get; set; } = NotificationLevel.Info;
        public string Tag { get; set; }
        public string Group { get; set; }
        public string LaunchArgs { get; set; }
    }
}