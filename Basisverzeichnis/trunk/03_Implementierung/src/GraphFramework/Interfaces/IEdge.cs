namespace GraphFramework.Interfaces
{
    /// <summary>
    /// An interface for classes that are able to connect 
    /// two objects of a class that implements the interface INode.
    /// Objects of classes that implement this interface can be added to 
    /// objects of classes that implement the interface IGraph.
    /// </summary>
    public interface IEdge : IGraphElement
    {
        /// <summary>
        /// The first INode to which the IEdge is connected.
        /// </summary>
        INode Node1 { get; }

        /// <summary>
        /// The second INode to which the IEdge is connected.
        /// </summary>
        INode Node2 { get; }
    }
}