using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts an usecasegraph to its variantCount attribute value
    /// </summary>
    public class UseCaseToVariantCountConverter : IValueConverter
    {
        /// <summary>
        /// converts a usecase graph to the its variant count
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UseCaseGraph usecasegraph = value as UseCaseGraph;
            if (usecasegraph == null)
            {
                return null;
            }

            int variantCount = usecasegraph.Edges.Count(e => e.GetAttributeByName("Description") != null);
            return variantCount < 1 ? 1 : variantCount;
        }

        /// <summary>
        /// converts the converted value back to its original type
        /// -- not supported here --> only 1 way binding is supported
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
