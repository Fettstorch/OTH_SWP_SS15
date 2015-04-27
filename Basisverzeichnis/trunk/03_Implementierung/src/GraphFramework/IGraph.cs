using System.Collections.Generic;

namespace GraphFramework
{
    public interface IGraph : IGraphElement
    {
        List<INode> Nodes { get; } 								    //contains all Nodes
        List<IEdge> Edges { get; } 								    //contains all Edges
        void addNode(INode newNode); 							    //adds Node to Nodes
        bool removeNode(INode nodeToBeRemoved);					    //removes Node from Nodes if found
        void addEdge(IEdge newEdge);								//adds Edge to Edges
        bool removeEdge(IEdge edgeToBeRemoved);					    //removes Edge from Edges if found
        List<IEdge> search(params IAttribute[] searchParameter); 	//returns all Edges in Edges that contain given Attribute or null
        IEdge next(); 											    //returns next Edge in Edges
        IEdge begin(); 											    //returns first Edge in Edges
    }
}