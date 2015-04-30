namespace GraphFramework.Interfaces
{
    public interface IEdge : IGraphElement
    {
        /// <summary>
        /// the first node to which the edge is connected
        /// </summary>
        INode Node1 { get; }

        /// <summary>
        /// the second node to which the edge is connected
        /// </summary>
        INode Node2 { get; }
    }
}