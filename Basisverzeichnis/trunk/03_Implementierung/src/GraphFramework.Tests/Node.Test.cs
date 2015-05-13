using System;
using NUnit.Framework;
using GraphFramework.Interfaces;

namespace GraphFramework.Tests
{
    public class NodeTest : BaseUnitTest
    {
        [TestCase(false, TestName = "AttributesNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that parameter attributes is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        [TestCase(true, TestName = "DefaultTest", 
            Description = "This is atest to check if the construction of a Node is working correctly. Expected is that the given attributes are set afterwards.\r\n\r\n")]
        public void ConstructorTest(bool setAttributes)
        {
            INode testEdge = new Node(setAttributes ? new IAttribute[0] : null);
            Assert.IsEmpty(testEdge.Attributes);
        }
    }
}
