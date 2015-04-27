using System.Collections.Generic;

namespace GraphFramework
{
    public interface INode : IGraphElement
    {
        List<IEdge> Edges { get; set; }	// contains all Edges of this Node
    }
}