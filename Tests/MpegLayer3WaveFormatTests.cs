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
    public class MpegLayer3WaveFormatTests
    {
        private WaveFormatExtensible wfx;
        private MpegLayer3WaveFormat mp3wfx;

        [SetUp]
        public void SetupTests()
        {
            wfx = new WaveFormatExtensible();
            wfx.FormatTag = 85;
            wfx.Channels = 2;
            wfx.SamplesPerSec = 8000;
            wfx.AverageBytesPerSecond = 500;
            wfx.BlockAlign = 1;
            wfx.BitsPerSample = 16;
            wfx.Size = 12;

            mp3wfx = new MpegLayer3WaveFormat();
        }

        [Test]
        public void WaveFormatExtensibleTest()
        {
            mp3wfx.WaveFormatExtensible = wfx;
            Assert.AreEqual(wfx, mp3wfx.WaveFormatExtensible);
        }

        [Test]
        public void WaveFormatExtensibleNullTest()
        {
            mp3wfx.WaveFormatExtensible = null;
            Assert.AreEqual(null, mp3wfx.WaveFormatExtensible);

        }

        [Test]
        public void WaveFormatExtensibleCoherencyTest()
        {
            /*
             * There are certain combinations of fields that are valid
             * and invalid in a MpegLayer3WaveFormat structure.
             * IE:
             *  mp3wfx.WaveFormatExtensible.Size must be 12;
             *  mp3wfx.Id must be 1
             *           
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * See the documentation for MPEGLAYER3WAVEFORMAT on msdn
             * http://msdn.microsoft.com/en-us/library/cc307970(VS.85).aspx
             */
            Assert.Ignore();
        }

        [Test]
        public void IdTest()
        {
            mp3wfx.Id = 1;
            Assert.AreEqual(1, mp3wfx.Id);

        }

        [Test]
        public void IdMinimumValueTest()
        {
            /*  
             * 1 is the smallest possible Id
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.Id = 1;
            Assert.Ignore();
        }

        [Test]
        public void IdMaximumValueTest()
        {
            /*  
             * 1 is the largest possible Id
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.Id = 1;
            Assert.Ignore();
        }

        [Test]
        public void BitratePaddingModeTest()
        {
            mp3wfx.BitratePaddingMode = 2;
            Assert.AreEqual(2, mp3wfx.BitratePaddingMode);

        }

        [Test]
        public void BitratePaddingModeMinimumValueTest()
        {
            /*  
             * 0 is the smallest possible BitratePaddingMode
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.BitratePaddingMode = -1;
            Assert.Ignore();
        }

        [Test]
        public void BitratePaddingModeMaximumValueTest()
        {
            /*  
             * 2 is the largest possible BitratePaddingMode
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.BitratePaddingMode = 3;
            Assert.Ignore();
        }

        [Test]
        public void BlockSizeTest()
        {
            mp3wfx.BlockSize = 500;
            Assert.AreEqual(500, mp3wfx.BlockSize);
        }

        [Test]
        public void BlockSizeMinimumValueTest()
        {
            /*  
             * 1 is the smallest possible BlockSize
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.BlockSize = 0;
            Assert.Ignore();
        }

        [Test]
        public void FramePerBlockTest()
        {
            mp3wfx.FramesPerBlock = 1;
            Assert.AreEqual(1, mp3wfx.FramesPerBlock);
        }

        [Test]
        public void FramePerBlockMinimumValueTest()
        {
            /*  
             * 1 is the smallest possible number of FramesPerBlock
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.FramesPerBlock = 0;
            Assert.Ignore();
        }

        [Test]
        public void CodecDelayTest()
        {
            mp3wfx.CodecDelay = 10;
            Assert.AreEqual(10, mp3wfx.CodecDelay);
        }

        [Test]
        public void CodecDelayMinimumValueTest()
        {
            /*  
             * 0 is the smallest possible number of CodecDelay
             * 
             * Code currently does not check for this but could be added for extra
             * robustness
             * 
             * Clamp or throw
             */
            ////mp3wfx.CodecDelay = -1;
            Assert.Ignore();
        }

        [Test]
        public void ToHexStringTest()
        {
            mp3wfx.WaveFormatExtensible = wfx;
            mp3wfx.Id = 1;
            mp3wfx.BitratePaddingMode = 2;
            mp3wfx.BlockSize = 1000;
            mp3wfx.FramesPerBlock = 1;
            mp3wfx.CodecDelay = 0;

            string s = mp3wfx.ToHexString();
            string expectedResult = "55000200401F0000F4010000010010000C00010002000000E80301000000";
            Assert.AreEqual(expectedResult, s);
        }

        [Test]
        public void ToStringTest()
        {
            mp3wfx.WaveFormatExtensible = wfx;
            mp3wfx.Id = 1;
            mp3wfx.BitratePaddingMode = 2;
            mp3wfx.BlockSize = 1000;
            mp3wfx.FramesPerBlock = 1;
            mp3wfx.CodecDelay = 0;

            string s = mp3wfx.ToString();
            string expectedResult = "MPEGLAYER3 WAVEFORMATEX FormatTag: 85, Channels: 2, SamplesPerSec: 8000, AvgBytesPerSec: 500, BlockAlign: 1, BitsPerSample: 16, Size: 12 ID: 1, Flags: 2, BlockSize: 1000, FramesPerBlock 1, CodecDelay 0";
            Assert.AreEqual(expectedResult, s);
        }
    }
}
