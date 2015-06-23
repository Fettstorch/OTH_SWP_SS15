using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// filters an attributes collection by removing the attribtues of type 'HiddenAttribute'
    /// </summary>
    public class AttributesConverter : IValueConverter
    {
        /// <summary>
        /// converts the attributes collection to another without hidden attributes
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<IAttribute> attributes = value as IEnumerable<IAttribute>;
            if (attributes == null) return null;

            List<IAttribute> filteredAttributes = new List<IAttribute>(attributes.Where(a => !(a is HiddenAttribute)));
            return filteredAttributes;
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
            throw new NotSupportedException();
        }
    }
}