using System.Collections.Generic;

public interface INode
{
	public List<IEdge> Edges { get; set; }	// contains all Edges of this Node
}