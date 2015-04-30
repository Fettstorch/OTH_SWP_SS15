using GraphFramework.Interfaces;

namespace GraphFramework
{
    /// <summary>
    /// Node class
    /// </summary>
    public class Node : GraphElement, INode
    {    
        public Node(params IAttribute[] attributes) : base(attributes) {}
    }
}