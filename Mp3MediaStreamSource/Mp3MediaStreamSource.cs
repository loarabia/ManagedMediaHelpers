//-----------------------------------------------------------------------
// <copyright file="Mp3MediaStreamSource.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
// This source is subject to the Microsoft Reciprocal License (Ms-RL)
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/reciprocallicense.mspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Media
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Threading;
    using MediaParsers;

    /// <summary>
    /// TODO FILL ME IN LATER
    /// </summary>
    public class Mp3MediaStreamSource : MediaStreamSource
    {
        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        private const int Id3Version1TagSize = 128;

        /// <summary>
        /// TODO FILL ME IN LATER
        /// </summary>
        private Stream audioStream;

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        private MediaStreamDescription audioStreamDescription;

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        private DispatcherTimer tempTimer;

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        private long currentFrameStartPosition;

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        private int currentFrameSize;

        /// <summary>
        ///  Initializes a new instance of the Mp3MediaStreamSource class.
        /// </summary>
        /// <param name="s"> TODO FILL ME IN LATERs</param>
        public Mp3MediaStreamSource(Stream s)
        {
            this.audioStream = s;

            // Temporary Workaroud for Beta 2
            this.tempTimer = new DispatcherTimer();
            this.tempTimer.Interval = TimeSpan.FromMilliseconds(250);
            this.tempTimer.Tick += new EventHandler(this._timer_Tick);
        }

        /// <summary>
        /// Gets the number of samples delivered to the MediaElement
        /// </summary>
        public long SamplesDelivered { get; private set; }

        /// <summary>
        /// Gets TODO FILLE ME  IN LATER
        /// </summary>
        public MpegLayer3WaveFormat Mp3WaveFormat { get; private set; }

        /// <summary>
        /// Parses the passed in MediaStream to find the first frame and signals
        /// to its parent MediaElement that it is ready to begin playback by calling
        /// ReportOpenMediaCompleted.
        /// </summary>
        protected override void OpenMediaAsync()
        {
            this.tempTimer.Start();
        }

        /// <summary>
        /// Parses the next sample from the requested stream and then calls ReportGetSampleCompleted
        /// to inform its parent MediaElement of the next sample.
        /// </summary>
        /// <param name="mediaStreamType"> TODO FILL ME IN LATER</param>
        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            Dictionary<MediaSampleAttributeKeys, string> emptyDict = new Dictionary<MediaSampleAttributeKeys, string>();
            MediaStreamSample audioSample = null;

            if (this.currentFrameStartPosition + this.currentFrameSize + Id3Version1TagSize >= this.audioStream.Length)
            {
                audioSample = new MediaStreamSample(
                    this.audioStreamDescription,
                    null,
                    0,
                    0,
                    0,
                    emptyDict);
                this.ReportGetSampleCompleted(audioSample);
            }
            else
            {
                audioSample = new MediaStreamSample(
                    this.audioStreamDescription,
                    this.audioStream,
                    this.currentFrameStartPosition,
                    this.currentFrameSize,
                    0,
                    emptyDict);
                this.ReportGetSampleCompleted(audioSample);

                MpegFrame nextFrame = new MpegFrame(this.audioStream);
                if (nextFrame.Version == 1 && nextFrame.Layer == 3)
                {
                    this.currentFrameStartPosition = this.audioStream.Position - 4;
                    this.currentFrameSize = nextFrame.FrameSize;
                }
                else
                {
                    throw new IndexOutOfRangeException("Frame Is Not MP3");
                }
            }
        }

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        protected override void CloseMedia()
        {
        }

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        /// <param name="diagnosticKind">
        ///  TODO FILL ME IN LATERs
        /// </param>
        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        /// <param name="seekToTime">
        ///  TODO FILL ME IN LATERs
        /// </param>
        protected override void SeekAsync(long seekToTime)
        {
            this.ReportSeekCompleted(seekToTime);
        }

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        /// <param name="mediaStreamDescription">
        ///  TODO FILL ME IN LATERs
        /// </param>
        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        /// <param name="sender">  TODO FILL ME IN LATERs</param>
        /// <param name="e">  TODO FILL ME IN LATERss</param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            Dictionary<MediaSourceAttributesKeys, string> mediaSourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            Dictionary<MediaStreamAttributeKeys, string> mediaStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            List<MediaStreamDescription> mediaStreamDescriptions = new List<MediaStreamDescription>();

            byte[] audioData = new byte[this.audioStream.Length];
            if (audioData.Length != this.audioStream.Read(audioData, 0, audioData.Length))
            {
                throw new Exception("ERROR"); // TODO IMPROVE
            }

            int result = BitTools.FindBitPattern(audioData, new byte[2] { 255, 240 }, new byte[2] { 255, 240 });
            this.audioStream.Position = result;
            MpegFrame mpegLayer3Frame = new MpegFrame(this.audioStream);
            if (mpegLayer3Frame.FrameSize <= 0)
            {
                throw new Exception("ERROR"); // TODO IMPROVE
            }

            WaveFormatExtensible wfx = new WaveFormatExtensible();
            this.Mp3WaveFormat = new MpegLayer3WaveFormat();
            this.Mp3WaveFormat.WaveFormatExtensible = wfx;

            this.Mp3WaveFormat.WaveFormatExtensible.FormatTag = 85;
            this.Mp3WaveFormat.WaveFormatExtensible.Channels = (short)((mpegLayer3Frame.Channels == Channel.SingleChannel) ? 1 : 2);
            this.Mp3WaveFormat.WaveFormatExtensible.SamplesPerSec = mpegLayer3Frame.SamplingRate;
            this.Mp3WaveFormat.WaveFormatExtensible.AverageBytesPerSecond = mpegLayer3Frame.Bitrate / 8;
            this.Mp3WaveFormat.WaveFormatExtensible.BlockAlign = 1;
            this.Mp3WaveFormat.WaveFormatExtensible.BitsPerSample = 0;
            this.Mp3WaveFormat.WaveFormatExtensible.Size = 12;

            this.Mp3WaveFormat.Id = 1;
            this.Mp3WaveFormat.BitratePaddingMode = 0;
            this.Mp3WaveFormat.FramesPerBlock = 1;
            this.Mp3WaveFormat.BlockSize = (short)mpegLayer3Frame.FrameSize;
            this.Mp3WaveFormat.CodecDelay = 0;

            mediaStreamAttributes[MediaStreamAttributeKeys.CodecPrivateData] = this.Mp3WaveFormat.ToHexString();
            this.audioStreamDescription = new MediaStreamDescription(MediaStreamType.Audio, mediaStreamAttributes);

            mediaStreamDescriptions.Add(this.audioStreamDescription);

            mediaSourceAttributes[MediaSourceAttributesKeys.Duration] = TimeSpan.FromMinutes(0).Ticks.ToString();

            this.ReportOpenMediaCompleted(mediaSourceAttributes, mediaStreamDescriptions);

            this.currentFrameStartPosition = result;
            this.currentFrameSize = mpegLayer3Frame.FrameSize;

            this.tempTimer.Stop();
        }
    }
}
