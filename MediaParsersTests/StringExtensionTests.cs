//-----------------------------------------------------------------------
// <copyright file="StringExtensionTests.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
// This source is subject to the Microsoft Public License (Ms-PL)
// See http://code.msdn.microsoft.com/ManagedMediaHelpers/Project/License.aspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MediaParsersTests
{
    using System;
    using System.Net;
    using ExtensionMethods;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void BasicTest()
        {
            string s = "DEADBEEF";
            string result = s.ToLittleEndian();
            Assert.AreEqual("EFBEADDE", result);
        }

        [TestMethod]
        public void EmptyInput()
        {
            string s = string.Empty;
            string result = s.ToLittleEndian();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void SmallestValidInput()
        {
            // No-op.
            string s = "AB";
            string result = s.ToLittleEndian();
            Assert.AreEqual(s, result);
        }

        [TestMethod]
        public void OddLengthInput()
        {
            string s = "ABC";
            string result = s.ToLittleEndian();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void NullInput()
        {
            string s = null;
            string result = s.ToLittleEndian();
        }

        [TestMethod]
        public void NonHexInput()
        {
            string s = "WXYZABCD";
            string result = s.ToLittleEndian();
            ////Assert.Inconclusive("Not a bug in the code but a place to add robustness checks");
            ////Assert.AreEqual(string.Empty, result);
        }
    }
}
