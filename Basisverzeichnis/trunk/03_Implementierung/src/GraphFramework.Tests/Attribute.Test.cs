#region Copyright information
// <summary>
// <copyright file="Attribute.Test.cs">Copyright (c) 2015</copyright>
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
    public class AttributeTest : BaseUnitTest
    {
        [TestCase("name", 1, typeof(int), TestName = "DefaultTest", 
            Description = "This is a test to check if the construction of an Attribute is working. " +
                          "It is expected, that the given Arguments are set in the new Attribute object " +
                          "and the type of the parameter Value is correctly found and set as member Type. \r\n\r\n")]
        [TestCase(null, 1, typeof(int), TestName = "NameNull", ExpectedException = typeof(ArgumentNullException), 
            Description = "This is a test to check if the case that the parameter name is null is handled \r\n" +
                          "Expected: ArgumentNullException. \r\n\r\n")]
        [TestCase("", 1, typeof(int), TestName = "NameEmpty", ExpectedException = typeof(ArgumentException),
            Description = "This is a test to check if the case that the parameter name is empty is handled \r\n" +
                          "Expected: ArgumentException. \r\n\r\n")]
        [TestCase("name", null, typeof(int), TestName = "ValueNull", ExpectedException = typeof(ArgumentNullException),
            Description = "This is a test to check if the case that the parameter val is null is handled \r\n" +
                          "Expected: ArgumentNullException. \r\n\r\n")]
        public void ConstructorTest(string name, object value, Type type)
        {
            IAttribute testAttribute = new Attribute(name, value);
            Assert.AreEqual(name, testAttribute.Name);
            Assert.AreEqual(value, testAttribute.Value);
            Assert.AreEqual(type, testAttribute.Type);
        }

        [TestCase("name", 1, 2, typeof(int), 2, TestName = "DefaultTest", 
            Description = "This is a test to check if a new Value can be set to Property Value. " +
                          "It is expected that the given value is set to the Property Value. \r\n\r\n")]
        [TestCase("name", 1, "name", typeof(int), 1, TestName = "DifferentTypeThanExisting", ExpectedException = typeof(InvalidOperationException),
            Description = "This is a test to check if the setting of a different value type is handled.\r\nExpected: InvalidOperationException. \r\n\r\n")]
        [TestCase("name", 1, null, typeof(int), 1, TestName = "SetNull", ExpectedException = typeof(NullReferenceException),
            Description = "This is a test for checking for the correct exception when null is set.\r\nExpected: NullReferenceException. \r\n\r\n")]
        public void ValueTest(string name, object value, object assignedValue, Type type, object expectedValue)
        {
            IAttribute testAttribute = new Attribute(name, value);
            testAttribute.Value = assignedValue;
            Assert.AreEqual(name, testAttribute.Name);
            Assert.AreEqual(expectedValue, testAttribute.Value);
            Assert.AreEqual(type, testAttribute.Type);
        }
    }
}
