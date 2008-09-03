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
using System.IO;

namespace ManagedMediaParsersTests
{
    [TestFixture]
    public class MpegFrameTests
    {

        private Stream s = new MemoryStream(new byte[4] { 255, 251, 50, 0 });
        private MpegFrame mf;

        [SetUp]
        public void SetupFixture()
        {
            mf = new MpegFrame(s);
        }

        [Test]
        public void VersionTest()
        {
            Assert.AreEqual(1, mf.Version);
        }

        [Test]
        public void LayerTest()
        {
            Assert.AreEqual(3, mf.Layer);
        }

    }
}
