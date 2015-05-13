using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Converters
{
    public class AttributeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IAttribute) || ((IAttribute)value).Type != typeof(string))
            {
                return null;
            }

            string attributeValue = (string) ((IAttribute) value).Value;
            return !string.IsNullOrWhiteSpace(attributeValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}