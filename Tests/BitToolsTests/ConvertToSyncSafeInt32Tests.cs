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
    public class ConvertToSyncSafeInt32Tests
    {
        byte[] dataArray;

        [SetUp]
        public void SetupTests()
        {
            dataArray = new byte[6] { 79, 13, 28, 0, 59, 128 };
        }

        [Test]
        public void Simple()
        {
            int result = BitTools.ConvertSyncSafeToInt32(dataArray, 1);
            Assert.AreEqual(27721787, result);
        }

        [Test]
        public void LargestValidSyncSafeIntValue()
        {
            dataArray = new byte[4] { 127, 127, 127, 127 };
            int result = BitTools.ConvertSyncSafeToInt32(dataArray, 0);
            Assert.AreEqual(268435455, result);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TooSmallData()
        {
            dataArray = new byte[2] { 128, 64 };
            int result = BitTools.ConvertSyncSafeToInt32(dataArray, 0);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InvalidStartIndexTooSmall()
        {
            int result = BitTools.ConvertSyncSafeToInt32(dataArray, -1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InvalidStartIndexTooBig()
        {
            int result = BitTools.ConvertSyncSafeToInt32(dataArray, dataArray.Length );
        }

        [Test]
        public void InvalidData()
        {
            //dataArray = new byte[4] { 128, 128, 128, 128 };
            //int result = BitTools.ConvertSyncSafeToInt32(dataArray, 0);
            Assert.Ignore();
            /*
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
        }

        [Test]
        public void InvalidStartIndexTooCloseToEndOfArray()
        {
            int result;
            try
            {
                result = BitTools.ConvertSyncSafeToInt32(dataArray, 2);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                result = BitTools.ConvertSyncSafeToInt32(dataArray, 3);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullData()
        {
            dataArray = null;
            BitTools.ConvertSyncSafeToInt32(null, 0);
        }
    }
}
