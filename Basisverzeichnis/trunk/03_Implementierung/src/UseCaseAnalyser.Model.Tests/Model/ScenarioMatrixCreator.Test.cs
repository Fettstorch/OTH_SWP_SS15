using System.Collections.Generic;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;
using NUnit.Framework;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Model.Tests.Model
{
    [TestFixture]
    class ScenarioMatrixCreatorTests
    {
        private INode[] mTestNodes;
        private UseCaseGraph mTestGraph;

        [SetUp]
        public virtual void OnTestStarted()
        {
            const string name = "Name";
            string index = NodeAttributes.NormalIndex.AttributeName();
            IAttribute[][] testAttributes =
            {
                new IAttribute[]{new Attribute(name, "A"), new Attribute(index, "A"),  
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.StartNode), },
                new IAttribute[]{new Attribute(name, "B"), new Attribute(index, "B"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.NormalNode) },
                new IAttribute[]{new Attribute(name, "C"), new Attribute(index, "C"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.NormalNode) },
                new IAttribute[]{new Attribute(name, "D"), new Attribute(index, "D"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.VariantNode) },
                new IAttribute[]{new Attribute(name, "E"), new Attribute(index, "E"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.JumpNode) },
                new IAttribute[]{new Attribute(name, "F"), new Attribute(index, "F"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.EndNode) },
            };

            mTestNodes = new INode[]
            {
                new Node(testAttributes[0]),
                new Node(testAttributes[1]),
                new Node(testAttributes[2]),
                new Node(testAttributes[3]),
                new Node(testAttributes[4]),
                new Node(testAttributes[5])
            };

            mTestGraph = new UseCaseGraph(new Attribute(name, "testGraph"));
            foreach (INode node in mTestNodes)
            {
                mTestGraph.AddNode(node);
            }

            mTestGraph.AddEdge(mTestNodes[0], mTestNodes[1]);//A->B
            mTestGraph.AddEdge(mTestNodes[1], mTestNodes[2]);//B->C
            mTestGraph.AddEdge(mTestNodes[2], mTestNodes[5]);//C->F
            mTestGraph.AddEdge(mTestNodes[2], mTestNodes[3]);//C->D
            mTestGraph.AddEdge(mTestNodes[3], mTestNodes[4]);//D->E
            mTestGraph.AddEdge(mTestNodes[4], mTestNodes[0]);//E->A
        }

        [Test, Description("DefaultTest to check if method is generally working.")]
        public void CreateScenarioMatrix_DefaultTest()
        {
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph);
            Assert.AreEqual(scenarios.Count(), 2);
        }

        [TearDown]
        public virtual void OnTestFinished()
        {

        }
    }
}
