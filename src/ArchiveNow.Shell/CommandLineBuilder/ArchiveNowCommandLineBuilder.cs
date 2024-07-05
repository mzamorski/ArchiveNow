using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ArchiveNow.Utils;

namespace ArchiveNow.Shell.CommandLineBuilder
{
    public class ArchiveNowCommandLineBuilder
    {
        private const string PathsArgument = "--paths";
        private const string ProfileNameArgument = "--profile-name";
        private const string ProfileFilePathArgument = "--profile-file";

        private readonly IDictionary<string, string> _commandMap = new Dictionary<string, string>();
        private readonly ISet<string> _paths = new HashSet<string>();

        public void AddPaths(string[] paths, bool validate = false)
        {
            foreach (var path in paths)
            {
                if (validate && !File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }

                _paths.Add(path);
            }
        }

        public void SetProfileName(string profileName)
        {
            _commandMap.AddOrUpdate(ProfileNameArgument, profileName);
        }

        public void SetProfileFilePath(string profileFilePath)
        {
            _commandMap.AddOrUpdate(ProfileFilePathArgument, profileFilePath);
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            var paths = _paths
                .Select(p => $"\"{p}\"")
                .Concat(" ");

            output.Append($"{PathsArgument} {paths}");


            foreach (var pair in _commandMap)
            {
                output.AppendLine($"{pair.Key} {pair.Value}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
