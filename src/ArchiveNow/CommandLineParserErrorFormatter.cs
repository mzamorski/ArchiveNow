using System;
using System.Collections.Generic;

using Fclp;

namespace ArchiveNow
{
    internal class CommandLineParserErrorFormatter : ICommandLineParserErrorFormatter
    {
        public string Format(ICommandLineParserError parserError)
        {
            return string.Empty;
            throw new NotImplementedException();
        }

        public string Format(IEnumerable<ICommandLineParserError> parserErrors)
        {
            return string.Empty;
            throw new NotImplementedException();
        }
    }
}