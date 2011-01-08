//-----------------------------------------------------------------------
// <copyright file="MpegFrameTests.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
// This source is subject to the Microsoft Public License (Ms-PL)
// See http://code.msdn.microsoft.com/ManagedMediaHelpers/Project/License.aspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MediaParsersTests
{
    using System;
    using System.IO;
    using System.Net;
    using MediaParsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MpegFrameTests : IDisposable
    {
        private Stream s = new MemoryStream(new byte[4] { 255, 251, 50, 0 });
        private MpegFrame mf;

        [TestInitialize]
        public void Setup()
        {
            this.s.Position = 0;
            this.mf = new MpegFrame(this.s);
        }

        [TestMethod]
        public void Version()
        {
            Assert.AreEqual(1, this.mf.Version);
        }

        [TestMethod]
        public void Layer()
        {
            Assert.AreEqual(3, this.mf.Layer);
        }

        [TestMethod]
        public void IsProtected()
        {
            Assert.AreEqual(false, this.mf.IsProtected);
        }

        [TestMethod]
        public void BitRateIndex()
        {
            Assert.AreEqual(3, this.mf.BitrateIndex);
        }

        [TestMethod]
        public void SamplingrateIndex()
        {
            Assert.AreEqual(0, this.mf.SamplingRateIndex);
        }

        [TestMethod]
        public void Padding()
        {
            Assert.AreEqual(1, this.mf.Padding);
        }

        [TestMethod]
        public void Channels()
        {
            Assert.AreEqual(Channel.Stereo, this.mf.Channels);
        }

        [TestMethod]
        public void Bitrate()
        {
            Assert.AreEqual(48000, this.mf.Bitrate);
        }

        [TestMethod]
        public void SamplingRate()
        {
            Assert.AreEqual(44100, this.mf.SamplingRate);
        }

        [TestMethod]
        public void FrameSize()
        {
            Assert.AreEqual(157, this.mf.FrameSize);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual(
                "FrameSize\t157\nBitRate\t48000\nSamplingRate44100\n",
                this.mf.ToString());
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.mf = null;
            }

            this.s.Close();
        }
        #endregion
    }
}
