#region Copyright information
// <summary>
// <copyright file="GraphElementToStringConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>15/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
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
        /// <summary>
        /// converts a graph element to a string by returining the name attribute of the graph element.
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
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
