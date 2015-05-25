#region Copyright information
// <summary>
// <copyright file="Edge.Test.cs">Copyright (c) 2015</copyright>
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

        [Test, Description("This is a test to check the dconstruction of an object. " +
                           "Expected is that two nodes and the attributes are correctly set afterwards. \r\n\r\n")]
        public void ConstructorTest()
        {
            IEdge testEdge = new Edge(testNode1, testNode2, new IAttribute[0]);
            Assert.AreEqual(testEdge.Node1, testNode1);
            Assert.AreEqual(testEdge.Node2, testNode2);
            Assert.IsEmpty(testEdge.Attributes);
        }

        [TestCase(false, true, true, TestName = "Node1Null", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if the case that parameter n1 is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(true, false, true, TestName = "Node2Null", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that parameter n2 is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(true, true, false, TestName = "AttributesNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that parameter attributes is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        public void ConstructorExceptionTest(bool setNode1, bool setNode2, bool setAttributes)
        {
            IEdge testEdge = new Edge(setNode1 ? testNode1 : null,
                setNode2 ? testNode2 : null, setAttributes ? new IAttribute[0] : null);
        }
    }
}
