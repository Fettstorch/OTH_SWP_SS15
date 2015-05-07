using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFramework.Interfaces;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    public class EdgeTest : BaseUnitTest
    {
        private static INode testNode1;
        private static INode testNode2;

        public override void OnTestStarted()
        {
            base.OnTestStarted();
            testNode1 = new Node(new IAttribute[0]);
            testNode2 = new Node(new IAttribute[0]);
        }

        [TestCase()]
        public void ConstructorTest()
        {
            IEdge testEdge = new Edge(testNode1, testNode2, new IAttribute[0]);
            Assert.AreEqual(testEdge.Node1, testNode1);
            Assert.AreEqual(testEdge.Node2, testNode2);
            Assert.IsEmpty(testEdge.Attributes);
        }

        [ExpectedException]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void ConstructorExceptionTest(bool setNode1, bool setNode2, bool setAttributes)
        {
            IEdge testEdge = new Edge(setNode1 ? testNode1 : null,
                setNode2 ? testNode2 : null, setAttributes ? new IAttribute[0] : null);
        }
    }
}
