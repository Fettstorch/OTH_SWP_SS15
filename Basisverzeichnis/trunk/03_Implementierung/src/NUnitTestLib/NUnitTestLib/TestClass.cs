using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTestLib
{
    public class TestClass
    {
        private int _myValue;

        public TestClass(int myValue)
        {
            _myValue = myValue;
        }

        public TestClass() : this (0)
        {
            
        }

        public int MyValue
        {
            get { return _myValue; }
            set { _myValue = value; }
        }


        public void IncrementValue()
        {
            _myValue++;
        }

        public void DecrementValue()
        {
            _myValue--;
        }

        public void DoSomethingWithMyValue(Action<TestClass> changeAction)
        {
            if (changeAction == null) throw new ArgumentNullException("changeAction");
            changeAction(this);
        }
    }
}
