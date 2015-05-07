using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using GraphFramework.Interfaces;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    public class GraphElementTest : BaseUnitTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ConstructorTest(int count)
        {
            IAttribute[] testAttributes = new IAttribute[count];
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            Assert.AreEqual(count, testElement.Attributes.Count());
        }

        [TestCase("name", 1)]
        public void AddAttributeTest(string name, object value)
        {
            IAttribute testAttribute = new Attribute(name, value);
            IGraphElement testElement = new Node(new IAttribute[0]);
            testElement.AddAttribute(testAttribute);
            Assert.AreEqual(1, testElement.Attributes.Count());
            Assert.AreEqual(testAttribute.Name, testElement.Attributes.First().Name);
            Assert.AreEqual(testAttribute.Type, testElement.Attributes.First().Type);
            Assert.AreEqual(testAttribute.Value, testElement.Attributes.First().Value);
        }

        [ExpectedException]
        [Test]
        public void AddAttributeExceptionTest()
        {
            IGraphElement testElement = new Node(new IAttribute[0]);
            testElement.AddAttribute(null);
        }

        [TestCase(3, "1")]
        public void RemoveAttributeTest_StringVersion(int count, string removeName)
        {
            IAttribute[] testAttributes = new IAttribute[count];      
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            testElement.RemoveAttribute(removeName);
            Assert.AreEqual(count-1, testElement.Attributes.Count());
            Assert.IsFalse(testElement.Attributes.Contains(testAttributes[int.Parse(removeName)]));
        }

        [ExpectedException]
        [TestCase(3, "5")]
        public void RemoveAttributeExceptionTest_StringVersion(int count, string removeName)
        {
            IAttribute[] testAttributes = new IAttribute[count];
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            testElement.RemoveAttribute(removeName);
        }

        [TestCase(3, 1)]
        public void RemoveAttributeTest_ObjectVersion(int count, int removeIndex)
        {
            IAttribute[] testAttributes = new IAttribute[count];
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            testElement.RemoveAttribute(testAttributes[removeIndex]);
            Assert.AreEqual(count - 1, testElement.Attributes.Count());
            Assert.IsFalse(testElement.Attributes.Contains(testAttributes[removeIndex]));
        }

        [ExpectedException]
        [TestCase(3)]
        public void RemoveAttributeExceptionTest_ObjectVersion(int count)
        {
            IAttribute[] testAttributes = new IAttribute[count];
            IAttribute removeAttribute = new Attribute("name", "abc");
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            testElement.RemoveAttribute(removeAttribute);
        }
    }
}
