using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFramework.Interfaces;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    public class AttributeTest : BaseUnitTest
    {
        [TestCase("name", 1, typeof(int))]
        public void ConstructorTest(string name, object value, Type type)
        {
            IAttribute testAttribute = new Attribute(name, value);
            Assert.AreEqual(name, testAttribute.Name);
            Assert.AreEqual(value, testAttribute.Value);
            Assert.AreEqual(type, testAttribute.Type);
        }

        [ExpectedException]
        [TestCase("", 1, typeof(int))]
        [TestCase(null, 1, typeof(int))]
        [TestCase("name", null, typeof(int))] 
        public void ConstructorExceptionTest(string name, object value, Type type)
        {
            IAttribute testAttribute = new Attribute(name, value);
        }

        [TestCase("name", 1, 2, typeof(int), 2)]
        public void ValueTest(string name, object value, object assignedValue, Type type, object expectedValue)
        {
            IAttribute testAttribute = new Attribute(name, value);
            testAttribute.Value = assignedValue;
            Assert.AreEqual(name, testAttribute.Name);
            Assert.AreEqual(expectedValue, testAttribute.Value);
            Assert.AreEqual(type, testAttribute.Type);
        }

        [ExpectedException]
        [TestCase("name", 1, "name", typeof(int), 1)]
        public void ValueExceptionTest(string name, object value, object assignedValue, Type type, object expectedValue)
        {
            IAttribute testAttribute = new Attribute(name, value);
            testAttribute.Value = assignedValue;
        }
    }
}
