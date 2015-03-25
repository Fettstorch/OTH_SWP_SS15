using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;


namespace NUnitTestLib
{
    public class Test
    {
        [Test]
        public void Test_1()
        {
            Assert.AreEqual(1,1);
        }


        [Test]
        public void Test_2()
        {
            Assert.AreEqual(2,2);
        }

        [Test]
        public void Test_3()
        {
            Assert.AreEqual(4, 2);
        }

    }
}
