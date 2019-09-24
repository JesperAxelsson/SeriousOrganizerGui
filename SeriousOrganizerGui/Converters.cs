using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SeriousOrganizerGui.Converters
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class SizeToStringConverter : BaseConverter, IValueConverter
    {
        const UInt64 KB = 1000;
        const UInt64 MB = KB * KB;
        const UInt64 GB = KB * KB * KB;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            var size = (UInt64)value;

            if (size > GB) return (size / GB) + " GB";
            if (size > MB) return (size / MB) + " MB";
            if (size > KB) return (size / KB) + " KB";

            return value + " B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NameToBrushConverter : BaseConverter, IValueConverter
    {
        static readonly string[] MovieExtentions = { ".mp4", ".wmv", ".m4v", ".avi", ".mkv", ".flv" };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string? input = (value as string)?.TrimEnd();
            if (input == null) return DependencyProperty.UnsetValue;

            if (MovieExtentions.Any(me => input.ToLowerInvariant().EndsWith(me, StringComparison.Ordinal)))
                return Brushes.Blue;

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


}
