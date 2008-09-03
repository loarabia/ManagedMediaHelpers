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

namespace ManagedMediaParsersTests
{
    [TestFixture]
    public class WaveFormatExtensibleTests
    {
        private WaveFormatExtensible wfx;

        [SetUp]
        public void SetupTests()
        {
            wfx = new WaveFormatExtensible();
        }

        [Test]
        public void FormatTagTest()
        {
            wfx.FormatTag = 1;
            Assert.AreEqual(1, wfx.FormatTag);   
        }

        [Test]
        public void FormatTagMaximumValueTest()
        {
            /* 
             * There is a maximum Format that has been defined for WFXs
             * should be in mmreg.h
             *  
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            Assert.Ignore();
        }

        [Test]
        public void FormatTagMinimumValueTest()
        {
            /*  
             * 0 is the smallest possible format ID
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             *
             * Clamp or throw
             */
            //// wfx.FormatTag = -1;
            Assert.Ignore();
        }

        [Test]
        public void ChannelsTest()
        {
            wfx.Channels = 1;
            Assert.AreEqual(1, wfx.Channels);
            
            wfx.Channels = 2;
            Assert.AreEqual(2, wfx.Channels);
        }

        [Test]
        public void ChannelsMaximumValueTest()
        {
            /*  
             * 2 is the largest number of channels
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////wfx.Channels = 3;
            Assert.Ignore();
        }

        [Test]
        public void ChannelsMinimumValueTest()
        {
            /*  
             * 1 is the smallest number of channels
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////wfx.Channels = 0;
            Assert.Ignore();
        }

        [Test]
        public void SamplesPerSecTest()
        {
            wfx.SamplesPerSec = 1000;
            Assert.AreEqual(1000, wfx.SamplesPerSec);
        }

        [Test]
        public void SamplesPerSecMinimumValueTest()
        {
            /*  
             * 0 is the smallest number of SamplesPerSecTest
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
             //// wfx.SamplesPerSec = -1;
             Assert.Ignore();
        }

        [Test]
        public void AverageBytesPerSecondTest()
        {
            wfx.AverageBytesPerSecond = 1000;
            Assert.AreEqual(1000, wfx.AverageBytesPerSecond);
        }

        [Test]
        public void AverageBytesPerSecondMinimumValueTest()
        {
            /*  
             * 0 is the smallest number of AverageBytesPerSecond
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            //// wfx.AverageBytesPerSecond = -1;
            Assert.Ignore();
        }

        [Test]
        public void BlockAlignTest()
        {
            wfx.BlockAlign = 1000;
            Assert.AreEqual(1000, wfx.BlockAlign);
        }

        [Test]
        public void BlockAlignMinimumValueTest()
        {
            /*  
             * 0 is the smallest number of BlockAlign
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            //// wfx.BlockAlign = -1;
            Assert.Ignore();
        }

        [Test]
        public void BitsPerSampleTest()
        {
            wfx.BitsPerSample = 16;
            Assert.AreEqual(16, wfx.BitsPerSample);
        }

        [Test]
        public void BitsPerSampleMinimumValueTest()
        {
            /*  
             * 1 is the smallest number of BitsPerSample
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            //// wfx.BitsPerSample = 0;
            Assert.Ignore();
        }

        [Test]
        public void SizeTest()
        {
            wfx.Size = 12;
            Assert.AreEqual(12, wfx.Size);
        }

        [Test]
        public void SizeMinimumValueTest()
        {
            /*  
             * 0 is the smallest number of Size
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            //// wfx.Size = -1;
            Assert.Ignore();
        }

        [Test]
        public void WaveFormatExtensibleCoherencyTests()
        {
            /*
             * There are certain combinations of fields that are valid
             * and invalid in a WaveFormatExtensible structure.
             * IE haveing Format = 0 means that Size = 0;
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * See the documentation for WAVEFORMATEX on msdn
             * http://msdn.microsoft.com/en-us/library/ms713497.aspx
             */
            Assert.Ignore();
        }

        [Test]
        public void ToHexStringTest()
        {
            wfx.FormatTag = 85;
            wfx.Channels = 2;
            wfx.SamplesPerSec = 8000;
            wfx.AverageBytesPerSecond = 500;
            wfx.BlockAlign = 1;
            wfx.BitsPerSample = 16;
            wfx.Size = 12;

            string s = wfx.ToHexString();
            string expectedResult = "55000200401F0000F4010000010010000C00";
            Assert.AreEqual(expectedResult, s);
        }


        [Test]
        public void ToStringTest()
        {
            wfx.FormatTag = 85;
            wfx.Channels = 2;
            wfx.SamplesPerSec = 8000;
            wfx.AverageBytesPerSecond = 500;
            wfx.BlockAlign = 1;
            wfx.BitsPerSample = 16;
            wfx.Size = 12;

            string s = wfx.ToString();
            string expectedResult = "WAVEFORMATEX FormatTag: 85, Channels: 2, SamplesPerSec: 8000, AvgBytesPerSec: 500, BlockAlign: 1, BitsPerSample: 16, Size: 12 ";
            Assert.AreEqual(expectedResult, s);
        }
    }
}
