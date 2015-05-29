#region Copyright information
// <summary>
// <copyright file="Graph.Test.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>04/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using GraphFramework.Interfaces;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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

        [TestCase(false, TestName = "DefaultTest", 
            Description = "This ia a test to check if the constructor is working. It is expected that this is creating a new Graph object.\r\n\r\n")]
        [TestCase(true, TestName = "AttributesNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter attributes is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        public void ConstructorTest(bool attributesNull)
        {
            IGraph testGraph = new Graph(attributesNull ? null : new IAttribute[0]);
            Assert.IsEmpty(testGraph.Attributes);
            Assert.IsEmpty(testGraph.Edges);
            Assert.IsEmpty(testGraph.Nodes);
        }

        [Test, Description("This is a test to check if adding a Node to an existing Graph is working. It is expected that the Node is added to the Graphs Nodes.\r\n\r\n")]
        public void AddNodeTest()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            INode testNode = new Node(new IAttribute[0]);
            testGraph.AddNode(testNode);
            Assert.AreEqual(1, testGraph.Nodes.Count());
        }

        [TestCase(0, TestName = "AddNull", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if adding null as a Node is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(2, TestName = "AddSameNodeTwice", ExpectedException = typeof(InvalidOperationException), 
            Description = "This is a test to check if adding the same Node twice is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        public void AddNodeExceptionTest(int count)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            INode testNode = count > 0 ? new Node(new IAttribute[0]) : null;
            int i = 0;
            do
            {
                testGraph.AddNode(testNode);
                i++;
            } while (i < count);
        }

        [TestCase(1, TestName = "DefaultTest", Result = 1, 
            Description = "This is a test to check if removing a Node from Graph is working. " +
                          "It is expected that the removed Node is not part of the Graph afterwards " +
                          "and that there is only one Edge left in the Graph.\r\n\r\n")]
        [TestCase(2, TestName = "RemoveMultipleNodes", Result = 0,
            Description = "This is a test to check if removing more than one Node from Graph is working. " +
                          "It is expected that the removed Nodes are not part of the Graph afterwards " +
                          "and that there are no Edges left in the Graph.\r\n\r\n")]
        public int RemoveNodeTest(int count)
        {
            IGraph testGraph = mTestMergeGraph;
            for (int i = 0; i < count; i++)
            {
                testGraph.RemoveNode(testGraph.Nodes.First());
            }
            Assert.AreEqual(3 - count, testGraph.Nodes.Count());          
            Assert.IsFalse(testGraph.Nodes.Contains(mTestNodelist[0]));
            return testGraph.Edges.Count();
        }

        [TestCase(2, TestName = "RemoveMultipleNodesAtOnce", Result = 0,
            Description = "This is a test to check if removing more than one Node from Graph in one call is working. " +
                          "It is expected that the removed Nodes are not part of the Graph afterwards " +
                          "and that there are no Edges left in the Graph.\r\n\r\n")]
        public int RemoveNodeTest_MultipleAtOnce(int count)
        {
            IGraph testGraph = mTestMergeGraph;
            testGraph.RemoveNode(mTestNodelist[0], mTestNodelist[1]);
            Assert.AreEqual(3 - count, testGraph.Nodes.Count());
            Assert.IsFalse(testGraph.Nodes.Contains(mTestNodelist[0]));
            return testGraph.Edges.Count();
        }

        [TestCase(0, TestName = "RemoveNull", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if removing null as a Node from the Graph is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(2, TestName = "RemoveNodeTwice", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if removing the same Node twice from the Graph is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
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

        [ExpectedException(typeof(InvalidOperationException))]
        [Test, Description("This is a test to check if removing a Node that is not part of the Graph from it is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        public void RemoveForeignNodeExceptionTest()
        {
            IGraph testGraph = mTestMergeGraph;
            testGraph.RemoveNode(new Node(new IAttribute[0]));
        }

        [TestCase(true, TestName = "ConnectSameNode", 
            Description = "This is a test to check if it is possible to connect a Node with itself/builidng a loop. " +
                          "It is expected that the added Edge has the same Node as Node1 and Node2.\r\n\r\n")]
        [TestCase(false, TestName = "ConnectTwoNodes", 
            Description = "This is a test to check if connecting two Nodes by an Edge is working. " +
                          "It is expected that the Nodes of the added Edge are not the same and not null.\r\n\r\n")]
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

        [TestCase(true, false, true, TestName = "FirstNodeNull", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if the case that the parameter node1 is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, true, true, TestName = "SecondNodeNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter node2 is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        [TestCase(true, true, false, TestName = "AttributesNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter attributes is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        public void AddEdgeExceptionTest(bool setNode1, bool setNode2, bool setAttributes)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            foreach (INode node in mTestNodelist)
            {
                testGraph.AddNode(node);
            }
            testGraph.AddEdge(setNode1 ? mTestNodelist[0] : null,
                setNode2 ? mTestNodelist[1] : null, setAttributes ? new IAttribute[0] : null);
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        public void AddEdgeV2Test_Null(IEdge edge)
        {
            IGraph testGraph = new Graph();
            testGraph.AddEdge(edge);
        }

        [TestCase(true, true, false, TestName = "DefaultTest")]
        [TestCase(false, true, false, TestName = "Node1NotPartOfGraph", ExpectedException = typeof(InvalidOperationException))]
        [TestCase(true, false, false, TestName = "Node2NotPartOfGraph", ExpectedException = typeof(InvalidOperationException))]
        [TestCase(true, true, true, TestName = "EdgeAlreadyPartOfGraph", ExpectedException = typeof(InvalidOperationException))]
        public void AddEdgeV2Test_Exceptions(bool hasNode1, bool hasNode2, bool isPartOfGraph)
        {
            IGraph newTestGraph = new Graph();
            newTestGraph.AddNode(mTestNodelist[0]);
            newTestGraph.AddNode(mTestNodelist[1]);
            IEdge testEdge = new Edge(hasNode1 ? mTestNodelist[0] : new Node(), hasNode2 ? mTestNodelist[1] : new Node());
            if(isPartOfGraph)
                newTestGraph.AddEdge(testEdge);
            newTestGraph.AddEdge(testEdge);
            Assert.IsTrue(newTestGraph.Edges.Count(t => t.Equals(testEdge)) == 1);
        }

        [Test, Description("This is a test to check if it is possible to remove one Edge from the Graph. " +
                           "It is expected that the Edge is not part of the Graph afterwards.\r\n\r\n")]
        public void RemoveEdgeTest()
        {
            IGraph testGraph = mTestMergeGraph;
            IEdge removedEdge = testGraph.Edges.First();
            testGraph.RemoveEdge(removedEdge);
            Assert.AreEqual(2, testGraph.Edges.Count());
            Assert.IsFalse(testGraph.Edges.Contains((removedEdge)));
        }

        [Test, Description("This is a test to check if it is possible to remove more than one Edge from the Graph in one call. " +
                           "It is expected that the Edges are not part of the Graph afterwards.\r\n\r\n")]
        public void RemoveEdgeTest_RemoveMultipleAtOnce()
        {
            IGraph testGraph = mTestMergeGraph;
            List<IEdge> removedEdges = new List<IEdge>();
            foreach (IEdge edge in testGraph.Edges)
            {
                removedEdges.Add(edge);
            }
            removedEdges.Remove(removedEdges.First());
            testGraph.RemoveEdge(removedEdges.ToArray());
            Assert.AreEqual(1, testGraph.Edges.Count());
            foreach (IEdge edge in removedEdges)
            {
                Assert.IsFalse(testGraph.Edges.Contains((edge)));
            }            
        }

        [TestCase(0, TestName = "RemoveNull", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if removing null from the Graph is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(4, TestName = "RemoveMoreEdgesThanExist", ExpectedException = typeof(InvalidOperationException), 
            Description = "This is a test to check if removing more Edges than are existing in the Graph is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
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

        [ExpectedException(typeof(InvalidOperationException))]
        [Test, Description("This is a test to check if removing an Edge that is not part of the Graph is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        public void RemoveForeignEdgeExceptionTest()
        {
            IGraph testGraph = mTestMergeGraph;
            testGraph.RemoveEdge(mTestEdgeList[0]);
        }

        [TestCase(0, TestName = "NoConnectionsBetweenNodes", Result = 3, 
            Description = "This is a test to check if all Nodes are returned if no Edges are part of the Graph. It is expected that all Nodes are returned.\r\n\r\n")]
        [TestCase(1, TestName = "OneConnectionBetweenTwoNodes", Result = 1, 
            Description = "This is a test to check if just the single Nodes are returned. It is expected that only one Node is returned.\r\n\r\n")]
        [TestCase(2, TestName = "TwoConnectionBetweenThreeNodes", Result = 0, 
            Description = "This is a test to check a Graph where all Nodes are connected. It is expected that no Nodes are returned.\r\n\r\n")]
        public int GetSingleNodes(int connections)
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
            return singleNodes.Count();
        }

        [Test, Description("This is a test to check if it is possible to merge two Graph objects, which share one Node. " +
                           "It is expected that all Nodes and Edges are contained in the new Graph and the Graphs are connected.\r\n\r\n")]
        public void MergeGraphTest_Simple_Connected()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(mTestNodelist[0]);
            IGraph newGraph = Graph.MergeGraphs(testGraph, mTestMergeGraph);
            foreach (INode node in newGraph.Nodes)
            {
                Assert.IsTrue(mTestMergeGraph.Nodes.Contains(node));
            }
            foreach (IEdge edge in newGraph.Edges)
            {
                Assert.IsTrue(mTestMergeGraph.Edges.Contains(edge));
            }
            Assert.AreEqual(mTestMergeGraph.Edges.Count(), newGraph.Edges.Count());
            Assert.AreEqual(mTestMergeGraph.Nodes.Count(), newGraph.Nodes.Count());
            Assert.AreEqual(0, newGraph.GetSingleNodes().Count());
        }

        [Test, Description("This is a test to check if it is possible to merge two Graph objects, which share no Node. " +
                           "It is expected that all Nodes and Edges are contained in the new Graph, the Graphs are not connected and there is one single Node.\r\n\r\n")]
        public void MergeGraphTest_Simple_NotConnected()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node());
            IGraph newGraph = Graph.MergeGraphs(testGraph, mTestMergeGraph);
            foreach (INode node in newGraph.Nodes)
            {
                Assert.IsTrue(mTestMergeGraph.Nodes.Concat(testGraph.Nodes).Contains(node));
            }
            foreach (IEdge edge in newGraph.Edges)
            {
                Assert.IsTrue(mTestMergeGraph.Edges.Concat(testGraph.Edges).Contains(edge));
            }
            Assert.AreEqual(mTestMergeGraph.Edges.Count(), newGraph.Edges.Count());
            Assert.AreEqual(mTestMergeGraph.Nodes.Count() + testGraph.Nodes.Count(), newGraph.Nodes.Count());
            Assert.AreEqual(newGraph.GetSingleNodes().Count(), testGraph.Nodes.Count());
        }

        [TestCase(true, false, TestName = "FirstGraphIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter graph1 is null is handled.\r\nExpected: ArgumentNullException\r\n\r\n")]
        [TestCase(false, true, TestName = "SecondGraphIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter graph2 is null is handled.\r\nExpected: ArgumentNullException\r\n\r\n")]
        public void MergeGraphTest_Simple_GraphNullExceptions(bool firstGraphNull, bool secondGraphNull)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(mTestNodelist[0]);
            IGraph newGraph = Graph.MergeGraphs(firstGraphNull ? null : testGraph, secondGraphNull ? null : mTestMergeGraph);
        }

        [Test, Description("This is a test to check if the merging of two Graph objects is working and the connecting Edge is added correctly. " +
                           "It is expected that there are all Nodes and Edges of the given Graph objects existing in the new Graph object and " +
                           "one new Edge connecting the given Nodes graphOneConnectionNode and graphTwoConnectionNode.\r\n\r\n")]
        public void MergeGraphTest_WithConnectionPoint()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            IGraph newGraph = Graph.MergeGraphs(testGraph, mTestMergeGraph, testGraph.Nodes.First(), mTestMergeGraph.Nodes.First(), new IAttribute[0]);
            foreach (INode node in mTestMergeGraph.Nodes)
            {
                Assert.IsTrue(newGraph.Nodes.Contains(node));
            }
            foreach (IEdge edge in mTestMergeGraph.Edges)
            {
                Assert.IsTrue(newGraph.Edges.Contains(edge));
            }
            Assert.AreEqual(mTestMergeGraph.Edges.Count() + 1, newGraph.Edges.Count());
            Assert.AreEqual(mTestMergeGraph.Nodes.Count() + 1, newGraph.Nodes.Count());
        }

        [TestCase(true, false, false, false, false, TestName = "FirstGraphIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the parameter graph1 is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, true, false, false, false, TestName = "SecondGraphIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the parameter graph2 is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, false, true, false, false, TestName = "FirstNodeIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the parameter graphOneConnectionNode is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, false, false, true, false, TestName = "SecondNodeIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the parameter graphTwoConnectionNode is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, false, false, false, true, TestName = "AttributesIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the parameter attributes is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        public void MergeGraphTest_WithConnectionPoint_NullExceptions(bool firstGraphNull, bool secondGraphNull, bool firstNodeNull, bool secondNodeNull, bool attributesIsNull)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            IGraph newGraph = Graph.MergeGraphs(firstGraphNull ? null : testGraph, secondGraphNull ? null : mTestMergeGraph,
                firstNodeNull ? null : testGraph.Nodes.First(), secondNodeNull ? null : mTestMergeGraph.Nodes.First(), attributesIsNull ? null : new IAttribute[0]);
        }

        [TestCase(true, false, TestName = "FirstNodeIsForeign", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if the parameter graphOneConnectionNode is not part of the Graph is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        [TestCase(false, true, TestName = "SecondNodeIsForeign", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if the parameter graphTwoConnectionNode is not part of the Graph is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        public void MergeGraphTest_WithConnectionPoint_ForeignNodeException(bool firstNodeForeign, bool secondNodeForeign)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            IGraph newGraph = Graph.MergeGraphs(testGraph, mTestMergeGraph,
                firstNodeForeign ? new Node() : testGraph.Nodes.First(), secondNodeForeign ? new Node() : mTestMergeGraph.Nodes.First(), new IAttribute[0]);
        }

        [Test, Description("This is a test to check if the adding of one Graph two another is working, when one Node is part of both Graphs. " +
                           "It is expected that the Nodes an Edges of the Graph that is added are also part of the calling Graph afterwards. " +
                           "The new GraphsElements need to be connected to the old ones in the calling Graph afterwards.\r\n\r\n")]
        public void AddGraphTest_Simple_Connected()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(mTestNodelist[0]);
            testGraph.AddGraph(mTestMergeGraph);
            foreach (INode node in testGraph.Nodes)
            {
                Assert.IsTrue(mTestMergeGraph.Nodes.Contains(node));
            }
            foreach (IEdge edge in testGraph.Edges)
            {
                Assert.IsTrue(mTestMergeGraph.Edges.Contains(edge));
            }
            Assert.AreEqual(mTestMergeGraph.Edges.Count(), testGraph.Edges.Count());
            Assert.AreEqual(mTestMergeGraph.Nodes.Count(), testGraph.Nodes.Count());
        }

        [Test, Description("This is a test to check if the adding of one Graph two another is working, when no Node is part of both Graphs. " +
                           "It is expected that the Nodes an Edges of the Graph that is added are also part of the calling Graph afterwards." +
                           "The GraphElements are not connected to the old Elements in the calling Graph.\r\n\r\n")]
        public void AddGraphTest_Simple_NotConnected()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node());
            testGraph.AddGraph(mTestMergeGraph);
            foreach (INode node in testGraph.Nodes)
            {
                Assert.IsTrue(mTestMergeGraph.Nodes.Concat(testGraph.Nodes).Contains(node));
            }
            foreach (IEdge edge in testGraph.Edges)
            {
                Assert.IsTrue(mTestMergeGraph.Edges.Concat(testGraph.Edges).Contains(edge));
            }
            Assert.AreEqual(mTestMergeGraph.Edges.Count(), testGraph.Edges.Count());
            Assert.AreEqual(mTestMergeGraph.Nodes.Count() + 1, testGraph.Nodes.Count());
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [Test, Description("This is a test to check if the case that the Graph that is to be added is null is handled.\r\n" +
                           "Expected: ArgumentNullException\r\n\r\n")]
        public void AddGraphTest_Simple_GraphNullException()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(mTestNodelist[0]);
            testGraph.AddGraph(null);
        }

        [Test, Description("This is a test to check if the adding of one Graph two another is working and if the Graphs are connected by a new Edge afterwards." +
                           "It is expected that all nodes and edges of the Graph that is to be added are also part of the calling Graph afterwards. " +
                           "The new Graphelements need to be connected to the old Graphelements by a new Edge.\r\n\r\n")]
        public void AddGraphTest_WithConnectionPoint()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            testGraph.AddGraph(mTestMergeGraph, testGraph.Nodes.First(), mTestMergeGraph.Nodes.First(), new IAttribute[0]);
            foreach (INode node in mTestMergeGraph.Nodes)
            {
                Assert.IsTrue(testGraph.Nodes.Contains(node));
            }
            foreach (IEdge edge in mTestMergeGraph.Edges)
            {
                Assert.IsTrue(testGraph.Edges.Contains(edge));
            }
            Assert.AreEqual(mTestMergeGraph.Edges.Count() + 1, testGraph.Edges.Count());
            Assert.AreEqual(mTestMergeGraph.Nodes.Count() + 1, testGraph.Nodes.Count());
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [Test, Description("This is a test to check if the case that the Graph that is to be added is null is handled.\r\n" +
                           "Expected: ArgumentNullException.\r\n\r\n")]
        public void AddGraphTest_WithConnectionPoint_GraphNullException()
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            testGraph.AddGraph(null, testGraph.Nodes.First(), null, new IAttribute[0]);
        }

        [TestCase(true, false, false, TestName = "FirstNodeIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter thisGraphConnectionNode is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, true, false, TestName = "SecondNodeIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter graphToAddConnectionNode is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, false, true, TestName = "AttributesIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter attributes is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        public void AddGraphTest_WithConnectionPoint_NodeNullException(bool firstNodeNull, bool secondNodeNull, bool attributesIsNull)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            testGraph.AddGraph(mTestMergeGraph, firstNodeNull ? null : testGraph.Nodes.First(), 
                secondNodeNull ? null : mTestMergeGraph.Nodes.First(), attributesIsNull ? null : new IAttribute[0]);
        }

        [TestCase(true, false, TestName = "FirstNodeIsForeign", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if the case that the parameter thisGraphConnectionNode is not part of the Graph is handled.\r\n" +
                          "Expected: InvalidOperationException.\r\n\r\n")]
        [TestCase(false, true, TestName = "SecondNodeIsForeign", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if the case that the parameter graphToAddConnectionNode is not part of the Graph is handled.\r\n" +
                          "Expected: InvalidOperationException.\r\n\r\n")]
        public void AddGraphTest_WithConnectionPoint_ForeignNodeException(bool firstNodeForeign, bool secondNodeForeign)
        {
            IGraph testGraph = new Graph(new IAttribute[0]);
            testGraph.AddNode(new Node(new Attribute("Name", "D")));
            testGraph.AddGraph(mTestMergeGraph, firstNodeForeign ? mTestMergeGraph.Nodes.First() : testGraph.Nodes.First(),
                secondNodeForeign ? testGraph.Nodes.First() : mTestMergeGraph.Nodes.First(), new IAttribute[0]);
        }

        [TestCase(0, TestName = "NoConnectionBetweenNodes", Result = 0, 
            Description = "This is a test to check if no Edges are returned if there are no in the Graph.\r\n\r\n")]
        [TestCase(1, TestName = "OneConnectionBetweenNodes", Result = 1,
            Description = "This is a test to check if the correct Edge is returned if there is one between the given Nodes.\r\n\r\n")]
        [TestCase(2, TestName = "TwoConnectionsBetweenNodes", Result = 2,
            Description = "This is a test to check if the all Edges are returned if there are more than one between the given Nodes.\r\n\r\n")]
        public int GetEdgesTest(int connections)
        {
            IGraph testGraph = new Graph();
            testGraph.AddNode(mTestNodelist[0]);
            testGraph.AddNode(mTestNodelist[1]);
            for (int i = 0; i < connections; i++)
            {
                testGraph.AddEdge(mTestNodelist[0], mTestNodelist[1]);
            }
            IEnumerable<IEdge> edges = testGraph.GetEdges(mTestNodelist[0], mTestNodelist[1]);
            foreach (IEdge edge in edges)
            {
                Assert.AreSame(edge.Node1, mTestNodelist[0]);
                Assert.AreSame(edge.Node2, mTestNodelist[1]);
            }
            return testGraph.Edges.Count();
        }

        [TestCase(true, false, TestName = "FirstNodeIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the first Node is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(false, true, TestName = "SecondNodeIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the second Node is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        public void GetEdgesExceptionTest(bool firstNodeNull, bool secondNodeNull)
        {
            IGraph testGraph = new Graph();
            testGraph.AddNode(mTestNodelist[0]);
            testGraph.AddNode(mTestNodelist[1]);
            testGraph.AddEdge(mTestNodelist[0], mTestNodelist[1]);
            IEnumerable<IEdge> edges = 
                testGraph.GetEdges(firstNodeNull ? null : mTestNodelist[0], 
                secondNodeNull ? null : mTestNodelist[1]);
        }
    }
}
