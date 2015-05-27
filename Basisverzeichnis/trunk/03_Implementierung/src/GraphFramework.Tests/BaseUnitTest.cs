#region Copyright information
// <summary>
// <copyright file="BaseUnitTest.cs">Copyright (c) 2015</copyright>
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
