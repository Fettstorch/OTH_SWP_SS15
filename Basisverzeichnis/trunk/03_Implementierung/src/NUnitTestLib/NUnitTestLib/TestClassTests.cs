using System.Collections.Generic;
using NUnit.Framework;

namespace NUnitTestLib
{
    internal class TestClassTests : BaseUnitTest
    {
        private TestClass _mTestClass;

        [Test]
        public void IncrementTest()
        {
            var startValue = _mTestClass.MyValue;
            _mTestClass.IncrementValue();
            var newValue = _mTestClass.MyValue;

            Assert.AreEqual(startValue + 1, newValue);
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(5)]
        public void SetValueTest(int setValue)
        {
            var oldValue = _mTestClass.MyValue;

            _mTestClass.MyValue = setValue;

            Assert.AreEqual(setValue >= 0 ? setValue : oldValue, _mTestClass.MyValue);
        }

        [TestCaseSource("Wrappers")]
        public void ChangeValueTest(IntWrapper newValue)
        {
            _mTestClass.DoSomethingWithMyValue(v => v.MyValue = newValue.Value);

            Assert.AreEqual(newValue.Value, _mTestClass.MyValue);
        }

        public IEnumerable<IntWrapper> Wrappers
        {
            get
            {
                for (var i = 0; i < 10; i++)
                {
                    yield return new IntWrapper(i);
                }
            }
        }

        [Test]
        [ExpectedException]
        public void ChangeValueNullTest()
        {
            _mTestClass.DoSomethingWithMyValue(null);
        
        }


        [TestCase(5)]
        public void IncrementInLoopTest(int loopCount)
        {
            var oldValue = _mTestClass.MyValue;

            for (int i = 0; i < loopCount; i++)
            {
                _mTestClass.IncrementValue();
            }

            Assert.AreEqual(oldValue + loopCount, _mTestClass.MyValue);

        }
        
        public override void OnTestStarted()
        {
            _mTestClass = new TestClass();
        }
    }
}
