using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UseCaseAnalyser.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? boolean = value as bool?;
            if (boolean == null) return null;

            Visibility result = boolean.Value ? Visibility.Visible : Visibility.Hidden;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}