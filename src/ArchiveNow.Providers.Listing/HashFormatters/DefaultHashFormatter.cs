using System;

namespace ArchiveNow.Providers.Listing.HashFormatters
{
    internal class DefaultHashFormatter : IHashFormatter
    {
        public string Format(byte[] hashBytes)
        {
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
