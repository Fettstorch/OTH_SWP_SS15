#region Copyright information
// <summary>
// <copyright file="UseCaseToVariantCountConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>17/06/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Linq;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts an usecasegraph to its variantCount attribute value
    /// </summary>
    public class UseCaseToVariantCountConverter : GenericValueConverter<UseCaseGraph,int>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override int Convert(UseCaseGraph source)
        {
            int variantCount = source.Nodes.Count(e =>
                e.Attribute(NodeAttributes.NodeType, false) != null                                                                            //first check if node has node type attribut
                && e.AttributeValue<UseCaseGraph.NodeTypeAttribute>(NodeAttributes.NodeType).Equals(UseCaseGraph.NodeTypeAttribute.JumpNode)   //afterwards check if node type is jump node
            );
            return variantCount < 1 ? 1 : variantCount;
        }
    }
}
