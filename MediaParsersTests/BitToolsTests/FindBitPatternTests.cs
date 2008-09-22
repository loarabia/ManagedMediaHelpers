//-----------------------------------------------------------------------
// <copyright file="FindBitPatternTests.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
// This source is subject to the Microsoft Public License (Ms-PL)
// See http://code.msdn.microsoft.com/ManagedMediaHelpers/Project/License.aspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
    public class FindBitPatternTests
    {

        byte[] dataArray;
        byte[] mask;
        byte[] pattern;

        [SetUp]
        public void Setup()
        {
            dataArray = new byte[20]
            {
                128, 56, 255, 33, 0,
                48, 101, 45, 97, 1,
                88, 53, 97, 58, 45,
                7, 9, 25, 79, 109
            };
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DataNull()
        {
            pattern = new byte[4] { 10, 10, 10, 10 };
            mask = new byte[4] { 10, 10, 10, 10 };
            BitTools.FindBitPattern(null, pattern, mask);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void PatternNull()
        {
            BitTools.FindBitPattern(dataArray, null, mask);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MaskNull()
        {
            BitTools.FindBitPattern(dataArray, pattern, null);
        }

        [Test]
        public void MaskAndPatternAreDifferentSizes()
        {
            mask = new byte[4] {128, 128, 128, 128 };
            pattern = new byte[3] { 36, 0, 72 };
            int result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(-1, result);

            pattern = new byte[5] { 7, 9, 25, 79, 109 };
            result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void PatternLargerThanData()
        {
            dataArray = new byte[5] { 1, 2, 3, 4, 5 };
            mask = new byte[6] { 128, 128, 128, 128, 128, 128 };
            pattern = new byte[6] { 1, 2, 3, 4, 5, 6 };
            int result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void EmptyPattern()
        {
            mask = new byte[4]{128, 128, 128, 128};
            int result = BitTools.FindBitPattern(dataArray, new byte[0] { }, mask);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void EmptyData()
        {
            mask = new byte[4] { 128, 128, 128, 128 };
            pattern = new byte[4] { 128, 128, 128, 128 };
            int result = BitTools.FindBitPattern(new byte[0] { },pattern, mask);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void EmptyMask()
        {
            pattern = new byte[4] { 128, 128, 128, 128 };
            int result = BitTools.FindBitPattern(dataArray, pattern, new byte[0] { });
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void PatternIsData()
        {
            dataArray = new byte[4] { 1, 2, 3, 4 };
            mask = new byte[4] { 255, 255, 255, 255 };
            pattern = new byte[4] { 1, 2, 3, 4 };
            int result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PatternAtEnd()
        {
            mask = new byte[1] { 255 };
            pattern = new byte[1] { 109 };
            int result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(19, result);
        }

        [Test]
        public void PatternAtBeginning()
        {
            mask = new byte[1] { 255 };
            pattern = new byte[1] { 128 };
            int result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PatternCrossingByteBoundary()
        {
            pattern = new byte[3] { 0, 24, 240 };
            mask = new byte[3] { 15, 223, 240 };
            int result = BitTools.FindBitPattern(dataArray, pattern, mask);
            Assert.AreEqual(0, result);
        }

        [Test,ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartIndexTooSmall()
        {
            BitTools.FindBitPattern(dataArray, pattern, mask, -1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartIndexTooLarge()
        {
            BitTools.FindBitPattern(dataArray, pattern, mask, dataArray.Length);
        }

        [Test]
        public void SearchLaterInArray()
        {
            int result = BitTools.FindBitPattern(dataArray, new byte[1] { 1 }, new byte[1] { 1 }, 7);
            Assert.AreEqual(7, result);

            result = BitTools.FindBitPattern(dataArray, new byte[1] { 1 }, new byte[1] { 1 }, 13);
            Assert.AreEqual(14, result);
        }
    }
}
