#region Copyright information
// <summary>
// <copyright file="TraverseLoopCountConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>23/06/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts an usecasegraph to its TraverseLoopCount attribute value
    /// </summary>
    public class TraverseLoopCountConverter : GenericValueConverter<UseCaseGraph,int>
    {
        private UseCaseGraph mConvertedUseCaseGraph;

        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override int Convert(UseCaseGraph source)
        {
            mConvertedUseCaseGraph = source;
            return mConvertedUseCaseGraph.AttributeValue<int>(UseCaseAttributes.TraverseLoopCount);
        }

        /// <summary>
        /// converts the target value of type TTarget back to the source value of type TSource
        /// </summary>
        /// <param name="target">value to be converted</param>
        /// <returns>the converted value</returns>
        public override UseCaseGraph ConvertBack(int target)
        {
            if (target < 1 || target > 2) return mConvertedUseCaseGraph;

            //  set its the TraverseLoopCount attribute value to the value set in gui.
            mConvertedUseCaseGraph.Attribute(UseCaseAttributes.TraverseLoopCount).Value = target;
            //  let the usecase graph recalculate its scenarios
            mConvertedUseCaseGraph.RecalculateScenarios();

            return mConvertedUseCaseGraph;
        }
    }
}