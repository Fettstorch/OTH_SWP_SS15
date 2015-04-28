using System.Collections.Generic;

namespace GraphFramework
{
    public interface IGraph : IGraphElement
    {
        /// <summary>
        /// contains all nodes
        /// </summary>
        List<INode> Nodes { get; }

        /// <summary>
        /// contains all edges
        /// </summary>
        List<IEdge> Edges { get; }

        /// <summary>
        /// adds the node to the node collection of the graph
        /// </summary>
        /// <param name="newNode">reference of the node to add</param>
        void AddNode(INode newNode);

        /// <summary>
        /// removes the node identified by its reference from the graph
        /// </summary>
        /// <param name="nodeToBeRemoved">reference of the node to remove</param>
        /// <returns>weather the remove was successful</returns>
        bool RemoveNode(INode nodeToBeRemoved);

        /// <summary>
        /// adds the edge to the edge collection of the graph
        /// </summary>
        /// <param name="newEdge">reference of the edge to add</param>
        void AddEdge(IEdge newEdge);

        /// <summary>
        /// removes the edge identified by its reference from the graph
        /// </summary>
        /// <param name="edgeToBeRemoved">reference of the edge to remove</param>
        /// <returns></returns>
        bool RemoveEdge(IEdge edgeToBeRemoved);

        /// <summary>
        /// searches edges by attributes
        /// </summary>
        /// <param name="searchParameter">references of the edge attributes to search for</param>
        /// <returns>the edges with the given attributes</returns>
        List<IEdge> Search(params IAttribute[] searchParameter);

        /// <summary>
        /// gets the next edge from the edge collection
        /// </summary>
        /// <returns>next edge</returns>
        IEdge Next();

        /// <summary>
        /// gets the first edge from the edge collection
        /// </summary>
        /// <returns>the first edge</returns>
        IEdge Begin();

    }
}