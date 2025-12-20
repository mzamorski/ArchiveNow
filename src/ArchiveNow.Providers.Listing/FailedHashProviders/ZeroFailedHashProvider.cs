using ArchiveNow.Providers.Listing.HashFormatters;

namespace ArchiveNow.Providers.Listing.FailedHashProviders
{
    internal class ZeroFailedHashProvider : IFailedHashProvider
    {
        private readonly int _hashCharSize;
        private readonly byte[] _zeroHashBytes;
        private readonly IHashFormatter _hashFormatter = new DefaultHashFormatter();

        public ZeroFailedHashProvider(int hashSize)
        {
            int hashSizeInBytes = hashSize / 8;

            _zeroHashBytes = new byte[hashSizeInBytes];
            _hashCharSize = hashSize / 8;
        }

        public string Get()
        {
            return _hashFormatter.Format(_zeroHashBytes);
        }
    }
}
