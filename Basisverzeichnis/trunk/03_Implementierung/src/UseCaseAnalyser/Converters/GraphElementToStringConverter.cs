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
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts a graph element to a readable string --> returns 'Name' attribute or 'no name'
    /// </summary>
    public class GraphElementToStringConverter : GenericValueConverter<IGraphElement, string>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override string Convert(IGraphElement source)
        {
            IAttribute nameAttribute = source.Attributes.ByName("Name", false);
            return string.Format("{0}: {1}", source.GetType().Name, nameAttribute == null ? "" : nameAttribute.Value);
        }
    }
}
