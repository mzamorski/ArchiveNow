using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ArchiveNow.Configuration.Storages;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public static class FileNameBuilderFactory
    {
        private static readonly IFileNameBuilder Default = new LeaveOriginalFileNameBuilder();

        private static readonly IDictionary<string, Func<string, IFileNameBuilder>> _map =
            new Dictionary<string, Func<string, IFileNameBuilder>>(StringComparer.OrdinalIgnoreCase)
            {
            { "Random", _ => new RandomFileNameBuilder() },
            { "AddDateTime", _ => new AddDateTimeFileNameBuilder() },
            { "LeaveOriginal", _ => new LeaveOriginalFileNameBuilder() },
            { "AddVersion", _ => new AddVersionFileNameBuilder(new PersistentPreferencesStorage()) },
            { "Rename", param => new RenameFileNameBuilder(param) },
            { "Default", _ => Default }
            };

        public static IFileNameBuilder Build(string input)
        {
            ParseInput(input, out string name, out string param);

            if (_map.TryGetValue(name, out Func<string, IFileNameBuilder> builderFunc))
            {
                return builderFunc(param);
            }
            else
            {
                return Default;
            }
        }

        private static void ParseInput(string input, out string name, out string param)
        {
            const string pattern = @"^\s*(\w+)(?:\s*\(\s*(.*?)\s*\))?\s*$";

            var match = Regex.Match(input, pattern);

            if (!match.Success)
            {
                throw new ArgumentException("Invalid format: " + input);
            }

            name = match.Groups[1].Value;
            param = match.Groups[2].Success ? match.Groups[2].Value : null;
        }
    }

    public interface IFileNameBuilderFactory
    {
        IFileNameBuilder Build(string name);
    }
}
