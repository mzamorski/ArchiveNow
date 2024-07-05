namespace ArchiveNow.Actions.Core.Contexts
{
    public class MailContext
    {
        public string Host { get; set; }

        public int? Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public string Subject { get; set; }
    }
}