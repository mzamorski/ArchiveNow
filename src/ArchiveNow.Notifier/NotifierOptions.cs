namespace ArchiveNow.Notifier
{
    public sealed class NotifierOptions
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Client { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public string Icon { get; set; }
    }
}
