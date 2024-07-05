using System;
using System.Globalization;
using System.IO;
using ArchiveNow.Utils.IO;

namespace ArchiveNow.Providers.Core.FileNameBuilders.Formatters
{
    public class DateTimeFileNameFormatter : IFileNameFormatter
    {
        private const string DefaultDateTimeFormat = "yyyyMMdd-HHmm";

        private IFormatProvider _formatProvider;

        public DateTimeFileNameFormatter(DateTime dateTime)
        {
            DateTime = dateTime;
            _formatProvider = new DateTimeFormatInfo();
        }

        public virtual DateTime DateTime { get; }

        public string Format(string fileName)
        {
            var name = fileName.GetName(out var extension);

            var dateTimeString = DateTime.ToString(DefaultDateTimeFormat);

            var newFileName = $"{name}_{dateTimeString}";

            if (string.IsNullOrWhiteSpace(extension))
            {
                return newFileName;
            }

            return Path.ChangeExtension(newFileName, extension);
        }
    }
}