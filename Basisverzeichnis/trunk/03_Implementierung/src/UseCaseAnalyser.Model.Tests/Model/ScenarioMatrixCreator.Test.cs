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
        const string Name = "Name";
        readonly string mIndex = NodeAttributes.NormalIndex.AttributeName();

        [SetUp]
        public virtual void OnTestStarted()
        {                       
            IAttribute[][] testAttributes =
            {
                new IAttribute[]{new Attribute(Name, "A"), new Attribute(mIndex, "A"),  
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.StartNode), },
                new IAttribute[]{new Attribute(Name, "B"), new Attribute(mIndex, "B"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.NormalNode) },
                new IAttribute[]{new Attribute(Name, "C"), new Attribute(mIndex, "C"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.NormalNode) },
                new IAttribute[]{new Attribute(Name, "D"), new Attribute(mIndex, "D"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.VariantNode) },
                new IAttribute[]{new Attribute(Name, "E"), new Attribute(mIndex, "E"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.JumpNode) },
                new IAttribute[]{new Attribute(Name, "F"), new Attribute(mIndex, "F"),
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

            mTestGraph = new UseCaseGraph(new Attribute(Name, "testGraph"));
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
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph, 1);
            Assert.AreEqual(2, scenarios.Count());
        }

        [Test, Description("DefaultTest to check if method is generally working with less variants.")]
        public void CreateScenarioMatrix_VariantTest()
        {
            mTestGraph.AddAttribute(new Attribute(UseCaseAttributes.TraverseVariantCount.AttributeName(), 0));
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph,1 );
            Assert.AreEqual(1, scenarios.Count());
        }

        [Test, Description("ForwardJump with all variants, one loop.")]
        public void CreateScenarioMatrix_ForwardJump()
        {
            IAttribute[] testAttributes = {new Attribute(Name, "G"), new Attribute(mIndex, "G"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.JumpNode) };
            INode testNode = new Node(testAttributes);
            mTestGraph.AddNode(testNode);
            mTestGraph.AddEdge(mTestNodes[1], testNode);
            mTestGraph.AddEdge(testNode, mTestNodes[5]);
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph,1);
            Assert.AreEqual(4, scenarios.Count());
        }

        [Test, Description("Second BackwardJump with all variants, one loop.")]
        public void CreateScenarioMatrix_BackwardJump()
        {
            IAttribute[] testAttributes = {new Attribute(Name, "G"), new Attribute(mIndex, "G"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.JumpNode) };
            INode testNode = new Node(testAttributes);
            mTestGraph.AddNode(testNode);
            mTestGraph.AddEdge(mTestNodes[1], testNode);
            mTestGraph.AddEdge(testNode, mTestNodes[0]);
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph,1);
            Assert.AreEqual(5, scenarios.Count());
        }

        [TestCase(2, TestName = "Two Traversions", Description = "Two Traversions to pass"), Description("BackwardJump with multiple loop traversions.")]
        [TestCase(3, TestName = "Three Traversions", Description = "Three Traversions to pass")]
        public void CreateScenarioMatrix_BackwardJump_Loops(int numTraversions)
        {
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph, numTraversions);
            Assert.AreEqual(numTraversions+1, scenarios.Count());
        }

        [TestCase(0, TestName = "No Variants"), Description("Test to check traversion of specific number of variants")]
        [TestCase(1, TestName = "One Variant")]
        public void CreateScenarioMatrix_BackwardJump_Variants(int numVariants)
        {
            mTestGraph.AddAttribute(new Attribute("Varianten-Traversierungs-Anzahl", numVariants));
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph);
            Assert.AreEqual(numVariants + 1, scenarios.Count());
        }

        [Test, Description("Two EndNodes with all variants and one loop.")]
        public void CreateScenarioMatrix_TwoEndNodes()
        {
            IAttribute[] testAttributes = {new Attribute(Name, "G"), new Attribute(mIndex, "G"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.EndNode) };
            INode testNode = new Node(testAttributes);
            mTestGraph.AddNode(testNode);
            mTestGraph.AddEdge(mTestNodes[1], testNode);
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph,1);
            Assert.AreEqual(4, scenarios.Count());
        }

        [Test, Description("Jump from EndNode is possible, therefore there should be the same number of scenarios as in CreateScenarioMatrix_BackwardJump().")]
        public void CreateScenarioMatrix_JumpFromEndNode()
        {
            IAttribute[] testAttributes = {new Attribute(Name, "G"), new Attribute(mIndex, "G"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.JumpNode) };
            INode testNode = new Node(testAttributes);
            mTestGraph.AddNode(testNode);
            mTestGraph.AddEdge(mTestNodes[5], testNode);
            mTestGraph.AddEdge(testNode, mTestNodes[0]);
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph,1);
            Assert.AreEqual(5, scenarios.Count());
        }

        [Test, Description("Jump from EndNode is possible, therefore there should be the same number of scenarios as in CreateScenarioMatrix_BackwardJump().")]
        public void CreateScenarioMatrix_JumpFromEndNode_withLoops()
        {
            IAttribute[] testAttributes = {new Attribute(Name, "G"), new Attribute(mIndex, "G"),
                    new Attribute(NodeAttributes.NodeType.AttributeName(), UseCaseGraph.NodeTypeAttribute.JumpNode) };
            INode testNode = new Node(testAttributes);
            mTestGraph.AddNode(testNode);
            mTestGraph.AddEdge(mTestNodes[5], testNode);
            mTestGraph.AddEdge(testNode, mTestNodes[0]);
            mTestGraph.RemoveNode(mTestNodes[3]);
            mTestGraph.RemoveNode(mTestNodes[4]);
            IEnumerable<IGraph> scenarios = ScenarioMatrixCreator.CreateScenarios(mTestGraph, 2);
            Assert.AreEqual(3, scenarios.Count());
        }

        [TearDown]
        public virtual void OnTestFinished()
        {

        }
    }
}
