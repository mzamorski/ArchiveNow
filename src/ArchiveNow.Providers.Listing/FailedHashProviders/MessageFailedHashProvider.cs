namespace ArchiveNow.Providers.Listing.FailedHashProviders
{
    internal class MessageFailedHashProvider : IFailedHashProvider
    {
        private const string Message = "<Failed to compute hash>";
        public string Get()
        {
            return Message;
        }
    }
}
