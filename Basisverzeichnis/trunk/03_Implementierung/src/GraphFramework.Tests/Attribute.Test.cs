using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    public class Attribute : BaseUnitTest
    {
        [TestCase("name", 1, typeof(int))]
        public void ConstructorTest(string name, object value, System.Type type)
        {
            GraphFramework.Attribute TestAttribute = new GraphFramework.Attribute(name, value);
            Assert.AreEqual(name, TestAttribute.Name);
            Assert.AreEqual(value, TestAttribute.Value);
            Assert.AreEqual(type, TestAttribute.Type);
        }

        [ExpectedException(typeof(System.Exception))]
        [TestCase("", 1, typeof(int))]
        [TestCase(null, 1, typeof(int))]
        [TestCase("name", null, typeof(int))] 
        public void ConstructorExceptionTest(string name, object value, System.Type type)
        {
            GraphFramework.Attribute TestAttribute = new GraphFramework.Attribute(name, value);
            Assert.AreEqual(name, TestAttribute.Name);
            Assert.AreEqual(value, TestAttribute.Value);
            Assert.AreEqual(type, TestAttribute.Type);
        }

        [TestCase("name", 1, "name", typeof(int), 1)]
        [TestCase("name", 1, 2, typeof(int), 2)]
        public void ValueTest(string name, object value, object assignedValue, System.Type type, object expectedValue)
        {
            GraphFramework.Attribute TestAttribute = new GraphFramework.Attribute(name, value);
            TestAttribute.Value = assignedValue;
            Assert.AreEqual(name, TestAttribute.Name);
            Assert.AreEqual(expectedValue, TestAttribute.Value);
            Assert.AreEqual(type, TestAttribute.Type);
        }
    }
}
