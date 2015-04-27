using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnitTestLib
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
