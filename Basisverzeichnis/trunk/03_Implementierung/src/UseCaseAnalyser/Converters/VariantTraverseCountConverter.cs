using System;
using System.Globalization;
using System.Windows.Data;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts an usecasegraph to its TraverseVariantCount attribute value
    /// </summary>
    public class VariantTraverseCountConverter : IValueConverter
    {
        private UseCaseGraph mConvertedUseCaseGraph;

        /// <summary>
        /// converts the usecasegraph to its TraverseVariantCount attribute value
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            mConvertedUseCaseGraph = value as UseCaseGraph;
            if (mConvertedUseCaseGraph == null)
            {
                return null;
            }

            int traverseVariantCount = mConvertedUseCaseGraph.AttributeValue<int>(UseCaseAttributes.TraverseVariantCount);
            return traverseVariantCount;
        }

        /// <summary>
        /// converts the TraverseVariantCount value to the usecasegraph
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? nullableTraverseVariantCount = value as int?;
            if (nullableTraverseVariantCount == null || nullableTraverseVariantCount.Value < 1)
            {
                return null;
            }

            //  set its the traverseCount attribute value to the value set in gui.
            mConvertedUseCaseGraph.Attribute(UseCaseAttributes.TraverseVariantCount).Value =
                nullableTraverseVariantCount.Value;
            //  let the usecase graph recalculate its scenarios
            mConvertedUseCaseGraph.RecalculateScenarios();

            return mConvertedUseCaseGraph;
        }
    }

    /// <summary>
    /// converts an usecasegraph to its TraverseLoopCount attribute value
    /// </summary>
    public class TraverseLoopCountConverter : IValueConverter
    {
        private UseCaseGraph mConvertedUseCaseGraph;

        /// <summary>
        /// converts the usecasegraph to its TraverseLoopCount attribute value
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            mConvertedUseCaseGraph = value as UseCaseGraph;
            if (mConvertedUseCaseGraph == null)
            {
                return null;
            }

            int traverseVariantCount = mConvertedUseCaseGraph.AttributeValue<int>(UseCaseAttributes.TraverseLoopCount);
            return traverseVariantCount;
        }

        /// <summary>
        /// converts the TraverseLoopCount value to the usecasegraph
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? nullableTraverseLoopCount = value as int?;
            if (nullableTraverseLoopCount == null || nullableTraverseLoopCount.Value < 1 || nullableTraverseLoopCount > 3)
            {
                return null;
            }

            //  set its the TraverseLoopCount attribute value to the value set in gui.
            mConvertedUseCaseGraph.Attribute(UseCaseAttributes.TraverseLoopCount).Value =
                nullableTraverseLoopCount.Value;
            //  let the usecase graph recalculate its scenarios
            mConvertedUseCaseGraph.RecalculateScenarios();

            return mConvertedUseCaseGraph;
        }
    }
}
