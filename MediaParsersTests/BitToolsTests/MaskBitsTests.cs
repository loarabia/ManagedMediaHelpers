//-----------------------------------------------------------------------
// <copyright file="MaskBitsTests.cs" company="Larry Olson">
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
    public class MaskBitsTests
    {
        byte[] dataArray;

        [SetUp]
        public void SetupTests()
        {
            dataArray = new byte[36]
            {
                79,  128, 28,  13,
                59,    0, 128, 128,
                128, 128, 128, 128,
                128, 128, 128, 128,
                128, 128, 128, 128,
                128, 128, 128, 128,
                128, 128, 128, 128,
                128, 128, 128, 128,
                128, 128, 128, 128
            };
        }

        [Test]
        public void  SimpleTest()
        {
            int result = BitTools.MaskBits(dataArray, 16, 8);
            Assert.AreEqual(28, result);
        }

        [Test]
        public void  LowerEdgeArrayTest()
        {
            int result = BitTools.MaskBits(dataArray,0,8);
            Assert.AreEqual(79, result);
        }

        [Test]
        public void  UpperEdgeOfArrayTest()
        {
            int result = BitTools.MaskBits(dataArray, 40, 8);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void  TooSmallSizeTest()
        {
            int result;
            
            try
            {
                result = BitTools.MaskBits(dataArray, 0, 0);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(1, 1);
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected Exception");
            }

            try
            {
                result = BitTools.MaskBits(dataArray, 0, -1);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(1, 1);
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected Exception");
            }

            try
            {
                result = BitTools.MaskBits(dataArray, 0, -8);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(1, 1);
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected Exception");
            }
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void  TooLargeSizeTest()
        {
            int result = BitTools.MaskBits(dataArray, 0, 33);
        }

        [Test]
        public void  InvalidFirstBitTest()
        {
            int result;
            try
            {
                // TOO Small
                result = BitTools.MaskBits(dataArray, -1, 8);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(1, 1);
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected Exception");
            }

            try
            {
                // TOO Large
                result = BitTools.MaskBits(dataArray, 48, 8);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(1, 1);
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected Exception");
            } 
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void  InvalidLastBitTest()
        {
            int result = BitTools.MaskBits(dataArray, 287, 2);
        }

        [Test]
        public void MaskOutFullSizedAlignedBitsTest()
        {
            int result = BitTools.MaskBits(dataArray, 240, 32);
            Assert.AreEqual(-2139062144, result);
            
            result = BitTools.MaskBits(dataArray, 8, 32);
            Assert.AreEqual(-2145645253, result);

            result = BitTools.MaskBits(dataArray, 16, 32);
            Assert.AreEqual(470629120, result);
        }

        [Test]
        public void MaskOutFullSizeNonAlignedBitsTest()
        {
            int result = BitTools.MaskBits(dataArray, 4, 32);
            Assert.AreEqual(-134102829,result);

        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullDataTest()
        {
            dataArray = null;
            BitTools.MaskBits(dataArray, 0, 8);
        }

    }
}
