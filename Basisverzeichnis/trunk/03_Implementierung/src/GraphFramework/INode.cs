using System.Collections.Generic;

namespace GraphFramework
{
    public interface INode : IGraphElement
    {
        /// <summary>
        /// contains all edges of the node
        /// </summary>
        List<IEdge> Edges { get; set; }
    }
}