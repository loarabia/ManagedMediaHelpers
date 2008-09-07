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
using System.IO;

namespace MediaParsersTests
{
    [TestFixture]
    public class MpegFrameTests
    {

        private Stream s = new MemoryStream(new byte[4] { 255, 251, 50, 0 });
        private MpegFrame mf;

        [SetUp]
        public void Setup()
        {
            s.Position = 0;
            mf = new MpegFrame(s);
        }

        [Test]
        public void Version()
        {
            Assert.AreEqual(1, mf.Version);
        }

        [Test]
        public void Layer()
        {
            Assert.AreEqual(3, mf.Layer);
        }

        [Test]
        public void IsProtected()
        {
            Assert.AreEqual(false ,mf.IsProtected);
        }

        [Test]
        public void BitRateIndex()
        {
            Assert.AreEqual(3, mf.BitrateIndex);
        }

        [Test]
        public void SamplingrateIndex()
        {
            Assert.AreEqual(0, mf.SamplingRateIndex);
        }

        [Test]
        public void Padding()
        {
            Assert.AreEqual(1 ,mf.Padding);
        }

        [Test]
        public void Channels()
        {
            Assert.AreEqual(Channel.Stereo, mf.Channels);
        }

        [Test]
        public void Bitrate()
        {
            Assert.AreEqual(48000,mf.Bitrate);
        }

        [Test]
        public void SamplingRate()
        {
            Assert.AreEqual(44100,mf.SamplingRate);
        }

        [Test]
        public void FrameSize()
        {
            Assert.AreEqual(157,mf.FrameSize);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual(
                "FrameSize\t157\nBitRate\t48000\nSamplingRate44100\n",
                mf.ToString());
        }
    }
}
