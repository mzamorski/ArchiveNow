using System.Collections.Generic;
using System.Text;

using Fclp;
using Fclp.Internals;

namespace ArchiveNow
{
    internal class CommandLineOptionFormatter : ICommandLineOptionFormatter
    {
        public string Format(IEnumerable<ICommandLineOption> options)
        {
            var builder = new StringBuilder();


            return builder.ToString();
        }
    }
}