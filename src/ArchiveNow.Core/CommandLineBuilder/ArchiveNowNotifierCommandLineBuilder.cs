using System.Collections.Generic;

namespace ArchiveNow.Core.CommandLineBuilder
{
    public class ArchiveNowNotifierCommandLineBuilder
    {
        private string _title;
        private string _message;
        private string _path;

        public ArchiveNowNotifierCommandLineBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public ArchiveNowNotifierCommandLineBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public ArchiveNowNotifierCommandLineBuilder WithPath(string path)
        {
            _path = path;
            return this;
        }

        /// <summary>
        /// Builds the command line string with properly escaped arguments.
        /// </summary>
        public override string ToString()
        {
            var args = new List<string>();

            if (!string.IsNullOrWhiteSpace(_title))
            {
                args.Add($"--title \"{EscapeArgument(_title)}\"");
            }

            if (!string.IsNullOrWhiteSpace(_message))
            {
                args.Add($"--message \"{EscapeArgument(_message)}\"");
            }

            if (!string.IsNullOrWhiteSpace(_path))
            {
                args.Add($"--path \"{EscapeArgument(_path)}\"");
            }

            return string.Join(" ", args);
        }

        private static string EscapeArgument(string arg)
        {
            if (string.IsNullOrEmpty(arg)) return string.Empty;

            // Escape double quotes for command line safety
            return arg.Replace("\"", "\\\"");
        }
    }
}
