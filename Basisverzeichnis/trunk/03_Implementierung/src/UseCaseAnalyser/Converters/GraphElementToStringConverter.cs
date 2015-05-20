using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts a graph element to a readable string --> returns 'Name' attribute or 'no name'
    /// </summary>
    public class GraphElementToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            IGraphElement graphElement = value as IGraphElement;
            if (graphElement == null)
            {
                throw new NotSupportedException();
            }

            IAttribute nameAttribute = graphElement.Attributes.FirstOrDefault(a => a.Name == "Name");
            return string.Format("{0}: {1}", value.GetType().Name, nameAttribute == null ? "<no name>" : nameAttribute.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
