using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFramework.Interfaces;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    public class GraphTest : BaseUnitTest
    {
        private INode[] mTestNodelist;
        private IEdge[] mTestEdgeList;
        private IGraph mTestMergeGraph;

        public override void OnTestStarted()
        {
            IAttribute[] nodeAttributes = new IAttribute[]
            {
                new Attribute("Name", "A"), 
                new Attribute("Name", "B"), 
                new Attribute("Name", "C")
            };
            mTestNodelist = new INode[]
            {
                new Node(nodeAttributes[0]), 
                new Node(nodeAttributes[1]),
                new Node(nodeAttributes[2])
            };
            mTestEdgeList = new IEdge[]
            {
                new Edge(mTestNodelist[0], mTestNodelist[1]),
                new Edge(mTestNodelist[2], mTestNodelist[1]),
                new Edge(mTestNodelist[2], mTestNodelist[0]), 
            };
            mTestMergeGraph = new Graph(new Attribute("Name", "TestMergeGraph"));

            foreach (INode node in mTestNodelist)
            {
                mTestMergeGraph.AddNode(node);
            }
            foreach (IEdge edge in mTestEdgeList)
            {
                mTestMergeGraph.AddEdge(edge.Node1, edge.Node2);
            }
            base.OnTestStarted();
        }

        [Test]
        public void ConstructorTest()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            Assert.IsEmpty(testGraph.Attributes);
            Assert.IsEmpty(testGraph.Edges);
            Assert.IsEmpty(testGraph.Nodes);
        }

        [Test]
        public void AddNodeTest()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            INode testNode = new Node(new IAttribute[0]);
            testGraph.AddNode(testNode);
            Assert.AreEqual(1, testGraph.Nodes.Count());
        }

        [ExpectedException]
        [TestCase(0)]
        [TestCase(2)]
        public void AddNodeExceptionTest(int count)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            INode testNode = count > 0 ? new Node(new IAttribute[0]) : null; 
            for (int i = 0; i < count; i++)
            {
                testGraph.AddNode(testNode);
            }
        }

        [Test]
        public void RemoveNodeTest()
        {
            IGraph testGraph = mTestMergeGraph;
            testGraph.RemoveNode(mTestNodelist[0]);
            Assert.AreEqual(2, testGraph.Nodes.Count());
            Assert.AreEqual(1, testGraph.Edges.Count());
            Assert.IsFalse(testGraph.Nodes.Contains(mTestNodelist[0]));
        }

        [ExpectedException]
        [TestCase(0)]
        [TestCase(4)]
        public void RemoveNodeExceptionTest(int count)
        {
            IGraph testGraph = mTestMergeGraph;
            if (count <= 0)
            {
                testGraph.RemoveNode(null);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    testGraph.RemoveNode(mTestNodelist[0]);
                }
            }
        }

        [ExpectedException]
        [Test]
        public void RemoveForeignNodeExceptionTest()
        {
            IGraph testGraph = mTestMergeGraph;
            testGraph.RemoveNode(new Node(new IAttribute[0]));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AddEdgeTest(bool loop)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            foreach (INode node in mTestNodelist)
            {
                testGraph.AddNode(node);
            }
            testGraph.AddEdge(mTestNodelist[0], loop ? mTestNodelist[0] : mTestNodelist[1]);
            Assert.AreEqual(1, testGraph.Edges.Count());
            Assert.AreEqual(mTestNodelist[0], testGraph.Edges.First().Node1);
            Assert.AreEqual(loop ? mTestNodelist[0] : mTestNodelist[1], testGraph.Edges.First().Node2);
        }

        [ExpectedException]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void AddEdgeExceptionTest(bool setNode1, bool setNode2)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            foreach (INode node in mTestNodelist)
            {
                testGraph.AddNode(node);
            }
            testGraph.AddEdge(setNode1 ? mTestNodelist[0] : null,
                setNode2 ? mTestNodelist[1] : null);
        }

        [Test]
        public void RemoveEdgeTest()
        {
            IGraph testGraph = mTestMergeGraph;
            IEdge removedEdge = testGraph.Edges.First();
            testGraph.RemoveEdge(removedEdge);
            Assert.AreEqual(2, testGraph.Edges.Count());
            Assert.IsFalse(testGraph.Edges.Contains((removedEdge)));
        }

        [ExpectedException]
        [TestCase(0)]
        [TestCase(4)]
        public void RemoveEdgeExceptionTest(int count)
        {
            IGraph testGraph = mTestMergeGraph;
            if (count <= 0)
            {
                testGraph.RemoveEdge(null);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    testGraph.RemoveEdge(testGraph.Edges.First());
                }
            }         
        }

        [ExpectedException]
        [Test]
        public void RemoveForeignEdgeExceptionTest()
        {
            IGraph testGraph = mTestMergeGraph;
            testGraph.RemoveEdge(mTestEdgeList[0]);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetSingleNodes(int connections)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            foreach (INode node in mTestNodelist)
            {
                testGraph.AddNode(node);
            }
            for (int i = 0; i < connections; i++)
            {
                testGraph.AddEdge(mTestNodelist[i], mTestNodelist[i+1]);
            }
            IEnumerable<INode> singleNodes = testGraph.GetSingleNodes();
            Assert.AreEqual(connections > 0 ? mTestNodelist.Count() - (connections + 1) : mTestNodelist.Count() - connections,
                singleNodes.Count());
        }

        [Test, Ignore]
        public void MergeGraphTest_Simple()
        {
            
        }

        [Test, Ignore]
        public void MergeGraphTest_WithConnectionPoint()
        {
            
        }

        [Test, Ignore]
        public void AddGraphTest_Simple()
        {
            
        }

        [Test, Ignore]
        public void AddGraphTest_WithConnectionPoint()
        {
            
        }

        [TestCase(0)]
        [TestCase(2)]
        public void GetEdgesTest(int connections)
        {
            IGraph tesGraph = mTestMergeGraph;
            for (int i = 0; i < connections; i++)
            {
                tesGraph.AddEdge(mTestNodelist[0], mTestNodelist[1]);
            }
            IEnumerable<IEdge> edges = tesGraph.GetEdges(mTestNodelist[0], mTestNodelist[1]);
            foreach (IEdge edge in edges)
            {
                Assert.AreSame(edge.Node1, mTestNodelist[0]);
                Assert.AreSame(edge.Node2, mTestNodelist[1]);
            }
            Assert.AreEqual(connections+1, edges.Count());
        }
    }
}
