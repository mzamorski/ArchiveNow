namespace ArchiveNow.Actions.Core.Contexts
{
    /// <summary>
    /// Context for sending archive to remote ArchiveNow instance.
    /// </summary>
    public class SendToArchiveNowContext
    {
        public string Secret { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}
