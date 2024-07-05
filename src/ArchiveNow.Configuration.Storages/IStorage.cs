using System;

namespace ArchiveNow.Configuration.Storages
{
    public interface IStorage : IDisposable
    {
        bool Exists(string key);

        string Get(string key);

        string GetOrEmpty(string key);

        void Store(string key, string value);
    }
}
