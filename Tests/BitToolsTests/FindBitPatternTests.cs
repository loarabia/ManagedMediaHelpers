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
using ManagedMediaParsers;

namespace ManagedMediaParsersTests.BitToolsTests
{
    [TestFixture]
    public class FindBitPatternTests
    {

        byte[] dataArray;

        [SetUp]
        public void Setup()
        {
            dataArray = new byte[20]
            {
                128, 56, 255, 33, 0,
                48, 101, 45, 97, 1,
                88, 53, 28, 58, 45,
                7, 9, 25, 79, 109
            };
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DataNull()
        {
            byte[] pattern = new byte[4] { 10, 10, 10, 10 };
            byte[] mask = new byte[4] { 10, 10, 10, 10 };
            BitTools.FindBitPattern(null, pattern, mask);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void PatternNull()
        {
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MaskNull()
        {
        }

        [Test]
        public void MaskAndPatternAreDifferentSizes()
        {
        }

        [Test]
        public void PatternLargerThanData()
        {
        }

        [Test]
        public void EmptyPattern()
        {
        }

        [Test]
        public void EmptyData()
        {
        }

        [Test]
        public void EmptyMask()
        {
        }

    }
}
