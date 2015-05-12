using GraphFramework.Interfaces;

namespace GraphFramework
{
    /// <summary>
    /// Node class
    /// </summary>
    public class Node : GraphElement, INode
    {
        /// <summary>
        /// Adds also attributes to the node
        /// </summary>
        /// <param name="attributes">the attributes you want to add</param>
        public Node(params IAttribute[] attributes) : base(attributes) { }
    }
}