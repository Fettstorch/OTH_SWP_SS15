using System;
using System.Linq;
using GraphFramework.Interfaces;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    public class GraphElementTest : BaseUnitTest
    {
        [TestCase(-1, TestName = "AttributesNull", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if the case that the parameter attributes is null is handled.\r\n" +
                          "Expected: ArgumentNullException.\r\n\r\n")]
        [TestCase(0, TestName = "NoAttributes", Result = 0, 
            Description = "This is a test to check if adding no attributes at construction is working. " +
                          "It is expected that no attributes are added afterwards.\r\n\r\n")]
        [TestCase(1, TestName = "OneAttributeAdded",Result = 1, 
            Description = "This is a test to check if adding one attribute at construction is working. " +
                          "It is expected that exactly one attributes are added afterwards.\r\n\r\n")]
        [TestCase(2, TestName = "TwoAttributesAdded", Result = 2, 
            Description = "This is a test to check if adding more than one attributes at construction is working. " +
                          "It is expected that two attributes are added afterwards.\r\n\r\n")]
        public int ConstructorTest(int count)
        {
            IAttribute[] testAttributes = count < 0 ? null : new IAttribute[count];
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            return testElement.Attributes.Count();
        }

        [TestCase("name", 1, false, false, false, false, TestName = "DefaultTest", 
            Description = "This is a test to check if adding attributes to an existing GraphElement is working. " +
                          "It is expected, that the one Attribute is really added.\r\n\r\n")]
        [TestCase("name", 1, false, false, false, true, TestName = "TestMultipleAttributes",
            Description = "This is a test to check if adding more than one attribute to an existing GraphElement is working. " +
                          "It is expected, that the ammount of Attributes that are given is really added.\r\n\r\n")]
        [TestCase("name", 1, true, false, false, false, TestName = "AttributeNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if adding an Attribute that is null is handled.\r\n Expected: ArgumentNullException.\r\n\r\n")]
        [TestCase("name", 1, false, true, false, false, TestName = "DoubleInsertionOfAttribute", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if adding an Attribute that is already part of this GraphElement is handled.\r\n " +
                          "Expected: InvalidOperationException.\r\n\r\n")]
        [TestCase("name", 1, false, false, true, false, TestName = "DoubleInsertionOfName", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if adding an Attribute that is not part of this GraphElement but its name is already existing is handled.\r\n " +
                          "Expected: InvalidOperationException.\r\n\r\n")]
        public void AddAttributeTest(string name, object value, bool attributeNull, bool addMultipleTimes, bool addSameName, bool addSecondAttribute)
        {
            IAttribute testAttribute = new Attribute(name, value);
            IGraphElement testElement = new Node(new IAttribute[0]);
            testElement.AddAttribute(attributeNull ? null : testAttribute);
            if (addMultipleTimes) testElement.AddAttribute(testAttribute);
            if(addSameName) testElement.AddAttribute(new Attribute(name, value));
            if(addSecondAttribute) testElement.AddAttribute(new Attribute(name + "1", value));
            Assert.AreEqual(addSecondAttribute ? 2 : 1, testElement.Attributes.Count());
            Assert.AreEqual(testAttribute.Name, testElement.Attributes.First().Name);
            Assert.AreEqual(testAttribute.Type, testElement.Attributes.First().Type);
            Assert.AreEqual(testAttribute.Value, testElement.Attributes.First().Value);
        }

        [TestCase(3, "1", TestName = "DefaultTest", Description = "This is a test to check if removing an existing Attribute from GraphElement by its name is working. +" +
                                                                  "It is expected that the Attribute is not part of the GraphElement afterwards.\r\n\r\n")]
        [TestCase(3, "5", TestName = "UnknownName", ExpectedException = typeof(InvalidOperationException), 
            Description = "This is a test to check if removing a name that is not existing in this GraphElement is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        [TestCase(3, null, TestName = "NameIsNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if removing a name that is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
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

        [TestCase(3, 1, TestName = "DefaultTest", Description = "This is a test to check if removing an existing Attribute from GraphElement by its reference is working. +" +
                                                                  "It is expected that the Attribute is not part of the GraphElement afterwards.\r\n\r\n")]
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

        [TestCase(3, false, TestName = "RemoveForeignAttribute", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if trying to remove an Attribute that is not part of this GraphElement is handled.\r\nExpected: InvalidOperationException.\r\n\r\n")]
        [TestCase(3, true, TestName = "RemoveNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if trying to remove an Attribute that is null is handled.\r\nExpected: ArgumentNullException.\r\n\r\n")]
        public void RemoveAttributeExceptionTest_ObjectVersion(int count, bool removeNull)
        {
            IAttribute[] testAttributes = new IAttribute[count];
            IAttribute removeAttribute = new Attribute("name", "abc");
            for (int i = 0; i < count; i++)
            {
                testAttributes[i] = new Attribute(i.ToString(), i);
            }
            IGraphElement testElement = new Node(testAttributes);
            testElement.RemoveAttribute(removeNull ? null : removeAttribute);
        }
    }
}
