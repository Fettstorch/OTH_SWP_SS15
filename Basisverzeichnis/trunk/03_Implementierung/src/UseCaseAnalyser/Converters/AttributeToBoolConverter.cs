#region Copyright information
// <summary>
// <copyright file="AttributeToBoolConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>13/05/2015</creationDate>
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
    /// converts an attribute to a boolean to determine weather the attribute is filled
    /// </summary>
    public class AttributeToBoolConverter : GenericValueConverter<IAttribute, bool>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override bool Convert(IAttribute source)
        {
            return !string.IsNullOrWhiteSpace(source.Type == typeof(string) ? (string)source.Value : source.Value.ToString());
        }
    }
}