using GraphFramework.Interfaces;

namespace GraphFramework
{
    public class Edge : GraphElement, IEdge
    {
        public Edge(INode node1, INode node2)
        {
            this.Node1 = node1;
            this.Node2 = node2;
        }

        public INode Node1 { get; private set; }
        public INode Node2 { get; private set; }
    }
}