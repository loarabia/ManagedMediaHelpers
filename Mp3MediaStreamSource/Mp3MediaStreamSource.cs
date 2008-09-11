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
    ///  TODO FILL ME IN LATER
    /// </summary>
    public enum MediaStreamSourceState
    {
        /// <summary>
        ///  Indicates that the stream is being made ready to pass to the media pipeline
        /// </summary>
        Setup,

        /// <summary>
        /// Indicates that the stream is ready and data is being passed to the media pipeline
        /// </summary>
        Processing,

        /// <summary>
        /// Indicates that at least one stream has had all samples passed to the media pipeline
        /// </summary>
        Finished
    }

    /// <summary>
    /// TODO FILL ME IN LATER
    /// </summary>
    public class Mp3MediaStreamSource : MediaStreamSource
    {
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
        /// Gets the state of the MediaStreamSource.
        /// Setup: Indicates that the stream is being made ready to pass to the media pipeline
        /// Processing: Indicates that the stream is ready and data is being passed to the media pipeline
        /// Finished: Indicates that at least one stream has had all samples passed to the media pipeline
        /// </summary>
        public MediaStreamSourceState MediaStreamSourceState { get; private set; }

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
            throw new NotImplementedException();
        }

        /// <summary>
        ///  TODO FILL ME IN LATER
        /// </summary>
        protected override void CloseMedia()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            Dictionary<MediaSourceAttributesKeys, string> mediaSourceAttributes = null;
            Dictionary<MediaStreamAttributeKeys, string> mediaStreamAttributes = null;
            List<MediaStreamDescription> mediaStreamDescriptions = null;

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
            else
            {
                WaveFormatExtensible wfx = new WaveFormatExtensible();
                this.Mp3WaveFormat = new MpegLayer3WaveFormat();
                this.Mp3WaveFormat.WaveFormatExtensible = wfx;

                this.Mp3WaveFormat.WaveFormatExtensible.FormatTag = 85;
                this.Mp3WaveFormat.WaveFormatExtensible.Channels = (short)((mpegLayer3Frame.Channels == Channel.SingleChannel) ? 1 : 0);
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

                mediaSourceAttributes[MediaSourceAttributesKeys.Duration] = "0";

                this.ReportOpenMediaCompleted(mediaSourceAttributes, mediaStreamDescriptions);
            }

            this.tempTimer.Stop();
        }
    }
}
