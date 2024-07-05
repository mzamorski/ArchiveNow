using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Caching;

namespace ArchiveNow.Configuration.Storages
{
    public class PersistentPreferencesStorage : IStorage
    {
        private const string PreferencesFileName = "ArchiveNow.preferences";

        private readonly IDictionary<string, string> _preferences = new Dictionary<string, string>();
        private readonly IsolatedStorageFile _storageFile;
        private readonly FileCache _cacheFile;

        public PersistentPreferencesStorage()
        {
            _storageFile = IsolatedStorageFile.GetStore((IsolatedStorageScope.User | IsolatedStorageScope.Assembly),
                null, null);

            Load();

            //_cacheFile = new FileCache(FileCacheManagers.Hashed) { };
            //_cacheFile["AddVersion"] = "1.1";
            //var version = _cacheFile["AddVersion"];
        }

        /// <summary>
        /// TODO: Dispose
        /// </summary>
        ~PersistentPreferencesStorage()
        {
            //Dispose();
        }

        private void Load()
        {
            using (var storageFileStream = new IsolatedStorageFileStream(PreferencesFileName, FileMode.OpenOrCreate, _storageFile))
            using (var reader = new StreamReader(storageFileStream))
            {
                while (!reader.EndOfStream)
                {
                    var keyLine = reader.ReadLine();
                    var valueLine = reader.ReadLine();
                    
                    if (keyLine == null)
                    {
                        continue;
                    }

                    _preferences.Add(keyLine, valueLine);
                }
            }
        }

        private void Save()
        {
            using (var storageFileStream = new IsolatedStorageFileStream(PreferencesFileName, FileMode.Truncate, _storageFile))
            using (var writer = new StreamWriter(storageFileStream))
            {
                foreach (var entry in _preferences)
                {
                    writer.WriteLine(entry.Key);
                    writer.WriteLine(entry.Value);
                }
            }
        }

        public bool Exists(string key)
        {
            return _preferences.ContainsKey(key);
        }

        public string Get(string key)
        {
            return _preferences[key];
        }

        public string GetOrEmpty(string key)
        {
            if (!_preferences.ContainsKey(key))
            {
                return string.Empty;
            }

            return _preferences[key];
        }

        public void Store(string key, string value)
        {
            _preferences[key] = value;
        }

        public void Dispose()
        {
            if (_storageFile != null)
            {
                Save();
                _storageFile.Dispose();
            }
        }
    }
}