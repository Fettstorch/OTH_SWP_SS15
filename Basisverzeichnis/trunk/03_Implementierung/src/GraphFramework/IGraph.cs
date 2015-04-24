using System.Collections.Generic;

public interface IGraph
{
	public List<INode> Nodes { get; } 								//contains all Nodes
	public List<IEdge> Edges { get; } 								//contains all Edges
	public void addNode(INode newNode); 							//adds Node to Nodes
	public bool removeNode(INode nodeToBeRemoved);					//removes Node from Nodes if found
	public void addEdge(IEdge newEdge);								//adds Edge to Edges
	public bool removeEdge(IEdge edgeToBeRemoved);					//removes Edge from Edges if found
	public List<IEdge> search(params IAttribute searchParameter); 	//returns all Edges in Edges that contain given Attribute or null
	public List<INode> search(params IAttribute searchParameter); 	//returns all Nodes in Nodes that contain given Attribute or null
	public IEdge next(); 											//returns next Edge in Edges
	public INode next(); 											//returns next Node in Nodes
	public IEdge begin(); 											//returns first Edge in Edges
	public INode begin(); 											//returns first Node in Nodes
}