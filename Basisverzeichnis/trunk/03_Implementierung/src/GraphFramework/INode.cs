using System.Collections.Generic;

public interface INode
{
	List<IEdge> Edges { get; set; }	// contains all Edges of this Node
}