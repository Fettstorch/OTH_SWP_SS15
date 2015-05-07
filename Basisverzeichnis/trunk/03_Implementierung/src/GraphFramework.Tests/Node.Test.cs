using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GraphFramework.Interfaces;

namespace GraphFramework.Tests
{
    public class NodeTest : BaseUnitTest
    {
        [Test]
        public void ConstructorTest()
        {
            INode testEdge = new Node(new IAttribute[0]);
            Assert.IsEmpty(testEdge.Attributes);
        }
    }
}
