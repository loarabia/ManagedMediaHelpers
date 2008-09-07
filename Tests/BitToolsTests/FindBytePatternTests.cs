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
using MediaParsers;

namespace MediaParsersTests.BitToolsTests
{
    [TestFixture]
    public class FindBytePatternTests
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
            dataArray = null;
            byte[] pattern = new byte[4] { 53, 28, 58, 45};
            BitTools.FindBytePattern(dataArray, pattern);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void PatternNull()
        {
            byte[] pattern = new byte[4] { 53, 28, 58, 45 };
            BitTools.FindBytePattern(dataArray, null);
        }

        [Test]
        public void Simple()
        {
            byte[] pattern = new byte[4] { 53, 28, 58, 45 };
            int result = BitTools.FindBytePattern(dataArray, pattern);
            Assert.AreEqual(11,result);

            pattern = new byte[5] { 53, 28, 58, 45, 7 };
            BitTools.FindBytePattern(dataArray, pattern);
            Assert.AreEqual(11, result);
        }

        [Test]
        public void PatternMatchAtBeginning()
        {
            byte[] pattern = new byte[1] { 128 };
            int result = BitTools.FindBytePattern(dataArray, pattern);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PatternMatchAtEnd()
        {
            byte[] pattern = new byte[1] { 109 };
            int result = BitTools.FindBytePattern(dataArray, pattern);
            Assert.AreEqual(19, result);
        }

        [Test]
        public void PatrialMatchAtEnd()
        {
            byte[] pattern = new byte[4] { 79, 109, 1, 2 };
            int result = BitTools.FindBytePattern(dataArray, pattern);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void PatternLargerThanData()
        {
            byte[] pattern = new byte[4] { 79, 109, 1, 2 };
            byte[] data = new byte[3] { 79, 109, 1 };
            int result = BitTools.FindBytePattern(dataArray, pattern);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void PatternSameSizeAsArray()
        {
            int result = BitTools.FindBytePattern(dataArray, dataArray);
            Assert.AreEqual(0, result);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartIndexTooSmall()
        {
            byte[] pattern = new byte[1]{128};
            BitTools.FindBytePattern(dataArray, pattern, -1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartIndexTooLarge()
        {
            byte[] pattern = new byte[1] { 128 };
            BitTools.FindBytePattern(dataArray, pattern, dataArray.Length);
        }

        [Test]
        public void SearchLaterInArray()
        {
            dataArray = new byte[4] { 128, 4, 128, 128 };
            byte[] pattern = new byte[1] { 128 };

            int result = BitTools.FindBytePattern(dataArray, pattern, 1);
            Assert.AreEqual(2, result);

            result = BitTools.FindBytePattern(dataArray, pattern, 3);
            Assert.AreEqual(3, result);
        }
    }
}
