using System;
using System.Collections.Generic;

using ArchiveNow.Configuration.Storages;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public static class FileNameBuilderFactory
    {
        private static readonly IFileNameBuilder Default = new LeaveOriginalFileNameBuilder();

        private static readonly IDictionary<string, Func<IFileNameBuilder>> _map = new Dictionary
            <string, Func<IFileNameBuilder>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Random", ()=> new RandomFileNameBuilder() },
            { "AddDateTime", ()=> new AddDateTimeFileNameBuilder() },
            { "LeaveOriginal", ()=> new LeaveOriginalFileNameBuilder() },
            { "AddVersion", ()=> new AddVersionFileNameBuilder(new PersistentPreferencesStorage()) },
            { "Rename", ()=> new RenameFileNameBuilder() },
            { "Default", ()=> Default }
        };

        public static IFileNameBuilder Build(string name)
        {
            return (_map.ContainsKey(name) == false)
                ? Default 
                : _map[name]();
        }
    }

    public interface IFileNameBuilderFactory
    {
        IFileNameBuilder Build(string name);
    }
}
