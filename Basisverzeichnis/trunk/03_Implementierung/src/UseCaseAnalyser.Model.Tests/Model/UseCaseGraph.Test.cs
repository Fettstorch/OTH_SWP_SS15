using System;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;
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

        [TestCase(false, Description = "DefaultTest of IGraph Constructor", TestName = "DefaultTest")]
        [TestCase(true, ExpectedException = typeof(InvalidOperationException), Description = "Test to check empty UseCaseGraphs Scenarios", TestName = "EmptyGraphScenarios")]
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

        [TestCase(false, false, ExpectedException = typeof(InvalidOperationException), TestName = "NoNameAttribute"
            , Description = "No NameAttribute should throw an Exception.\r\nExpected: InvalidOperationException.")]
        [TestCase(true, false, TestName = "DefaultTest", Description = "DefaultTest to check if method is working.")]
        [TestCase(true, true, ExpectedException = typeof(InvalidOperationException), TestName = "MoreThanOneNameAttribute",
            Description = "When more than one NameAttribute is present there should be an Excpetion. Should be handled by GraphElement.\r\nExpected: InvalidOperationException.")]
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

        [TestCase(false, ExpectedException = typeof(InvalidOperationException), TestName = "NoNodesPresent", Description = "Test if it throws an Exception if Scenarios cannot be created.\r\nExpected: InvalidOperationException.")]
        [TestCase(true, TestName = "DefaultTest", Description = "DefaultTest to see if property returns valid values.")]
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
