using System;
using NUnit.Framework;

namespace GraphFramework.Tests
{
    [TestFixture]
    public class BaseUnitTest
    {
        [SetUp]
        public virtual void OnTestStarted()
        {

        }

        [TearDown]
        public virtual void OnTestFinished()
        {

        }
    }
}
