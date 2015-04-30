using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    public class Graph : IGraph
    {
        public IEnumerable<IAttribute> Attributes { get; private set; }
        public void AddAttribute(IAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public void RemoveAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveAttribute(IAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INode> Nodes { get; private set; }
        public IEnumerable<IEdge> Edges { get; private set; }
        public void AddNode(INode node, params IAttribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(INode node)
        {
            throw new NotImplementedException();
        }

        public void AddEdge(INode n1, INode n2, params IAttribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdge(IEdge edge)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INode> GetSingleNodes()
        {
            throw new NotImplementedException();
        }

        public void AddGraph(IGraph g2)
        {
            throw new NotImplementedException();
        }

        public void AddGraph(IGraph g2, INode n1, INode n2, params IAttribute[] attributes)
        {
            throw new NotImplementedException();
        }
    }
}
