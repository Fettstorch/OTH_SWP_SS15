using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using GraphFramework;
using NUnit.Framework;
using UseCaseAnalyser.Model.Model;
using Attribute = GraphFramework.Attribute;

namespace UseCaseAnalyser.Model.Tests.Model
{
    [TestFixture]
    class UseCaseGraphTests
    {
        [SetUp]
        public virtual void OnTestStarted()
        {
        }

        [TestCase(false)]
        [TestCase(true, ExpectedException = typeof(InvalidOperationException))]
        public void UseCaseGraph_ConstructorTest(bool testScenarios)
        {
            UseCaseGraph testGraph = new UseCaseGraph();
            Assert.IsEmpty(testGraph.Attributes);
            Assert.IsEmpty(testGraph.Edges);
            Assert.IsEmpty(testGraph.Nodes);
            if (testScenarios)
            {
                Assert.IsEmpty(testGraph.Scenarios);
            }
        }

        [TestCase(false, false, ExpectedException = typeof(InvalidOperationException))]
        [TestCase(true, false)]
        [TestCase(true, true, ExpectedException = typeof(InvalidOperationException))]
        public void UseCaseGraph_ToStringTest(bool initialize, bool doubleName)
        {
            UseCaseGraph testGraph = initialize
                ? new UseCaseGraph(new Attribute("Name", "testGraph"))
                : new UseCaseGraph();
            if (doubleName)
            {
                testGraph.AddAttribute(new Attribute("Name", "testGraph"));
            }
            Assert.AreEqual("testGraph", testGraph.ToString());
        }

        [TestCase(false, ExpectedException = typeof(InvalidOperationException))]
        [TestCase(true)]
        public void UseCaseGraph_ScenariosTest(bool initialize)
        {
            UseCaseGraph testGraph = new UseCaseGraph(new Attribute("Name", "testGraph"));
            if (initialize)
            {
                testGraph.AddNode(new Node(new Attribute("Name", "A"), 
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.StartNode),
                    new Attribute(NodeAttributes.NormalIndex.AttributeName(), "A")));
                testGraph.AddNode(new Node(new Attribute("Name", "B"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.EndNode),
                    new Attribute(NodeAttributes.NormalIndex.AttributeName(), "B")));
                testGraph.AddEdge(testGraph.Nodes.First(), testGraph.Nodes.Last());
            }
            IGraph[] scenarios = testGraph.Scenarios.ToArray();
        }

        [TearDown]
        public virtual void OnTestFinished()
        {

        }
    }
}
