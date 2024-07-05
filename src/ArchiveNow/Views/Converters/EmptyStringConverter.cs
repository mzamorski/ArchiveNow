using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ArchiveNow.Views.Converters
{
    public class EmptyStringConverter : MarkupExtension, IValueConverter
    {
        public string EmptyStringValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.EmptyStringValue = parameter as string;

            return string.IsNullOrEmpty(value as string) ? parameter : value;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return string.Equals(value as string, this.EmptyStringValue) ? string.Empty : value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
