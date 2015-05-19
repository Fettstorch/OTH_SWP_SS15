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
            IAttribute attribute = value as IAttribute;

            if (attribute == null)
            {
                return null;
            }

            string attributeValue = attribute.Type == typeof(string) ? (string) attribute.Value : attribute.Value.ToString();
            return !string.IsNullOrWhiteSpace(attributeValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}