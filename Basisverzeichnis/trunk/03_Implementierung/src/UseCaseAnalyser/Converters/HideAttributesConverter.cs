#region Copyright information
// <summary>
// <copyright file="HideAttributesConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>23/06/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Collections.Generic;
using System.Linq;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// removes the hidden attributes from the attribute enumerable
    /// </summary>
    public class HideAttributesConverter : GenericValueConverter<IEnumerable<IAttribute>, IAttribute[]>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override IAttribute[] Convert(IEnumerable<IAttribute> source)
        {
            return source.Where(a => !(a is HiddenAttribute)).ToArray();
        }
    }
}