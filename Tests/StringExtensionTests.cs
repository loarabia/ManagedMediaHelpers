using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NUnit.Framework;
using ExtensionMethods;

namespace ManagedMediaParsersTests
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        public void BasicTest()
        {
            string s = "DEADBEEF";
            string result = s.ToLittleEndian();
            Assert.AreEqual("EFBEADDE", result);
        }

        [Test]
        public void EmptyInput()
        {
            string s = string.Empty;
            string result = s.ToLittleEndian();
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void SmallestValidInput()
        {
            //No-op.
            string s = "AB";
            string result = s.ToLittleEndian();
            Assert.AreEqual(s, result);
        }

        [Test]
        public void OddLengthInput()
        {
            string s = "ABC";
            string result = s.ToLittleEndian();
            Assert.AreEqual(string.Empty, result);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NullInput()
        {
            string s = null;
            string result = s.ToLittleEndian();
        }

        [Test]
        public void NonHexInput()
        {
            string s = "WXYZABCD";
            string result = s.ToLittleEndian();
            Assert.Ignore("Not a bug in the code but a place to add robustness checks");
            ////Assert.AreEqual(string.Empty, result);
        }
    }
}
