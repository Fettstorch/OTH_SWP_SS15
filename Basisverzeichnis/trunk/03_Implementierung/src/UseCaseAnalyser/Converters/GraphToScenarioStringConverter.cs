using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Converters
{
    public class GraphToScenarioStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            IGraph scenarioGraph = value as IGraph;
            if (scenarioGraph == null)
            {
                throw new NotSupportedException();
            }

            IAttribute nameAttribute = scenarioGraph.Attributes.FirstOrDefault(a => a.Name == "Name");
            return string.Format("{0}: {1}", value.GetType().Name, nameAttribute == null ? "<no name>" : nameAttribute.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
