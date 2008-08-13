/******************************************************************************
 * (c) Copyright Microsoft Corporation.
 * This source is subject to the Microsoft Reciprocal License (Ms-RL)
 * See http://www.microsoft.com/resources/sharedsource/licensingbasics/reciprocallicense.mspx
 * All other rights reserved.
 ******************************************************************************/
using System;
using System.IO;

namespace ManagedMediaParsers
{
    /// <summary>
    /// A partial implementation of an MPEG audio frame as specified by IS0/IEC
    /// 13818-3 and ISO/IEC 11172-3.
    /// </summary>
    /// <remarks>
    /// The primary purpose of this class is to represent an Mpeg 1 Layer 3
    /// file or MP3 file for short. Many of the features not explicitly needed
    /// for audio rendering are omitted from the implementation.
    /// 
    /// Data on this format is readily discoverable in many books as well as by
    /// searching for "MP3 Frame" in your favorite search engine. As always,
    /// Wikipedia is well stocked in all of these areas as well.
    /// </remarks>
    public class MpegFrame
    {
        private const int SYNC_VALUE = 2047;    // Frame Sync is 12 1s
        private const int FRAME_HEADER_SIZE = 4;// MP3 Headers are 4 Bytes long

        //TODO: Cannot make pointers (reference types) consts.
        // What is the right way to make this data one const item ?
        private static int[,] BITRATE_INDEX = new int[,]
            {   {0,32,64,96,128,160,192,224,256,288,320,352,384,416,448,-1},
                {0,32,48,56,64,80,96,112,128,160,192,224,256,320,384,-1},
                {0,32,40,48,56,64,80,96,112,128,160,192,224,256,320,-1},
                {0,32,48,56,64,80,96,112,128,144,160,176,192,224,256,-1},
                {0,8,16,24,32,40,48,56,64,80,96,112,128,144,160,-1}
            };

        //TODO: Cannot make pointers (reference types) consts.
        // What is the right way to make this data one const item ?
        private static int[,] SAMPLNIG_RATE_INDEX = new int[,]
            {   {44100,48000,32000,-1},
                {22050,24000,16000,-1},
                {11025,12000,8000,-1}
            };

        /**********************************************************************
         * FILE DATA- data which comes directly from the MP3 header.
         *********************************************************************/
        #region File Data

        /// <summary>
        /// Version of the MPEG standard this frame conforms to.
        /// MPEG 1, MPEG 2, or MPEG 2.5
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// The layer of complexity used in this frame.
        /// Layer 1, 2, or 3.
        /// </summary>
        public int Layer { get; private set; }

        /// <summary>
        /// Indicates whether or not the frame is protected by a
        /// Cyclic Redundancy Check (CRC). If true, then a 16 bit
        /// CRC follows the header.
        /// </summary>
        public bool IsProtected { get; private set; }

        /// <summary>
        /// Index into the bitrate table as defined in the MPEG spec.
        /// </summary>
        public int BitrateIndex { get; private set; }

        /// <summary>
        /// Index into the samplingrate table as defined in the MPEG spec.
        /// </summary>
        public int SamplingRateIndex { get; private set; }

        /// <summary>
        /// The number of additional bytes of padding in this frame.
        /// </summary>
        public int Padding { get; private set; }
        
        /// <summary>
        /// The output channel used to encode this frame.
        /// </summary>
        public Channel Channels { get; private set; }
        #endregion

        /**********************************************************************
         * DERIVED DATA - data which is calculated from data in the header.
         *********************************************************************/
        #region Derived Data

        /// <summary>
        /// The number of bits per second the raw audio is compressed into.
        /// </summary>
        public int BitRate
        {
            get
            {
                switch (Version)
                {
                    case 1: // Version 1.0
                        switch (Layer)
                        {
                            case 1: // MPEG 1 Layer 1
                                return BITRATE_INDEX[0, BitrateIndex] * 1000;
                            case 2: // MPEG 1 Layer 2
                                return BITRATE_INDEX[1, BitrateIndex] * 1000;
                            case 3: // MPEG 1 Layer 3 (MP3)
                                return BITRATE_INDEX[2, BitrateIndex] * 1000;
                            default: // MPEG 1 LAYER ERROR
                                return -2;
                        }
                    case 2: // Version 2.0
                    case 3: // Version 2.5 in reality
                        switch (Layer)
                        {
                            case 1: // MPEG 2 or 2.5 Layer 1
                                return BITRATE_INDEX[3, BitrateIndex] * 1000;
                            case 2: // MPEG 2 or 2.5 Layer 2
                            case 3: // MPEG 2 or 2.5 Layer 3
                                return BITRATE_INDEX[4, BitrateIndex] * 1000;
                            default: // Mpeg 2 LAYER ERROR
                                return -2;
                        }
                    default: // VERSION ERROR
                        return -2;
                }
            }
        }
        
        /// <summary>
        /// The number of samples per second in the frame.
        /// </summary>
        public int SamplingRate
        {
            get
            {
                switch (Version)
                {
                    case 1: // MPEG 1
                        return SAMPLNIG_RATE_INDEX[0, SamplingRateIndex];
                    case 2: // MPEG 2
                        return SAMPLNIG_RATE_INDEX[1, SamplingRateIndex];
                    case 3: // MPEG 2.5
                        return SAMPLNIG_RATE_INDEX[2, SamplingRateIndex];
                    default:
                        return -1; // RESERVED
                }
            }
        }

        /// <summary>
        /// The number of bytes in the frame.
        /// </summary>
        public int FrameSize
        {
            get
            {
                switch (Layer)
                {
                    case 1:
                        return (12 * BitRate / SamplingRate + Padding) * 4; 
                    case 2:
                    case 3:
                        return 144 * BitRate / SamplingRate + Padding;
                    default:
                        return -1;
                }
            }
        }
        #endregion

        /// <summary>
        /// Converts the MpegFrame into a human readable form.
        /// </summary>
        /// <returns>
        /// A textual representation of the MpegFrame.
        /// </returns>
        public override string ToString()
        {
            string s = "";
            s += "FrameSize\t" + FrameSize + "\n";
            s += "BitRate\t" + BitRate + "\n";
            s += "SamplingRate" + SamplingRate + "\n";
            return s;
        }

        /// <summary>
        /// Creates an MpegFrame from a stream.
        /// </summary>
        /// <param name="stream">
        /// A stream with its position at the SyncPoint of the header.
        /// </param>
        public MpegFrame(Stream stream)
        {
            long startPostion = stream.Position;
            byte[] mp3FrameHeader = new byte[FRAME_HEADER_SIZE];

            // Guard against a read error
            if (stream.Read(mp3FrameHeader, 0, FRAME_HEADER_SIZE) != FRAME_HEADER_SIZE)
            {
                goto cleanup;
            }

            Array.Reverse(mp3FrameHeader);

            // Sync
            int value = BitTools.MaskBits(mp3FrameHeader, 21, 11);
            if (!(value == SYNC_VALUE))
            {
                goto cleanup;
            }

            Version = ParseVersion(mp3FrameHeader);
            Layer = ParseLayer(mp3FrameHeader);
            IsProtected = BitTools.MaskBits(mp3FrameHeader, 16, 1) == 1 ? false : true;
            BitrateIndex = BitTools.MaskBits(mp3FrameHeader, 12, 4);
            SamplingRateIndex = BitTools.MaskBits(mp3FrameHeader, 10, 2);
            Padding = BitTools.MaskBits(mp3FrameHeader, 9, 1);
            //Private Bit = BitTools.MaskBits(_mp3FrameHeader,8,1); USELESS
            Channels = ParseChannel(mp3FrameHeader);
            // Joint Mode = ParseJoitMode(_mp3FrameHeader); Not used by  Mp3MSS
            // CopyRight = BitTools.MaskBits(_mp3FrameHeader,3,1); Not used by Mp3MSS
            // Original = BitTools.MaskBits(_mp3FrameHeader,2,1); Not used by Mp3MSS
            // Emphasis = ParseEmphasis(_mp3FrameHeader); Not used by Mp3MSS

            return;
        cleanup:
            stream.Position = startPostion;
            mp3FrameHeader = null;
            return;
        }

/******************************************************************************
 * ParseVersion
 * 
 * Summary:
 * Parses the version of the MPEG standard this frame header conforms to from
 * the frame header.
 * 
 * Input:
 * The 4 byte header for this frame.
 * 
 * Output:
 * The version of the MPEG standard this frame conforms to.
 * 1 = Mpeg 1
 * 2 = Mpeg 2
 * 3 = Mpeg 2.5
 *****************************************************************************/
        private static int ParseVersion(byte[] mp3FrameHeader)
        {
            int version;
            int versionValue = BitTools.MaskBits(mp3FrameHeader, 19, 2);

            switch (versionValue)
            {
                case 3:
                    version = 1;
                    break;
                case 2:
                    version = 2;
                    break;
                case 0:
                    version = 3;
                    break;
                default:
                    version = -1;   // ERROR
                    break;
            }
            return version;
        }


/******************************************************************************
 * ParseLayer
 * 
 * Summary:
 * Parses which complexity layer of the MPEG standard this frame conforms to
 * from the frame header.
 * 
 * Input:
 * The 4 byte header for this frame.
 * 
 * Output:
 * The complexity layer this frame conforms to.
 *****************************************************************************/
        private static int ParseLayer(byte[] mp3FrameHeader)
        {
            int layer;
            int layerValue = BitTools.MaskBits(mp3FrameHeader, 17, 2);

            switch (layerValue)
            {
                case 3:
                    layer = 1;
                    break;
                case 2:
                    layer = 2;
                    break;
                case 1:
                    layer = 3;
                    break;
                default:
                    layer = -1;
                    break;
            }
            return layer;
        }

/******************************************************************************
 * ParseChannel
 * 
 * Summary:
 * Parses the audio output mode of this frame's audio data.
 * 
 * Input:
 * The 4 byte header for this frame.
 * 
 * Output:
 * The audio output mode of this frame's audio data.
 *****************************************************************************/
        private static Channel ParseChannel(byte[] mp3FrameHeader)
        {
            Channel channel;
            int channelValue = BitTools.MaskBits(mp3FrameHeader, 6, 2);

            switch (channelValue)
            {
                case 3:
                    channel = Channel.SingleChannel;
                    break;
                case 2:
                    channel = Channel.DualChannel;
                    break;
                case 1:
                    channel = Channel.JointStereo;
                    break;
                case 0:
                    channel = Channel.Stereo;
                    break;
                default:
                    channel = Channel.SingleChannel; // ERROR CASE
                    break;
            }
            return channel;
        }

    }

    /// <summary>
    /// Reproduction mode of given audio data. Typically maps to the number of
    /// output devices used to reproduce it.
    /// 
    /// Stereo: independent audio typically output to 2 speakers and is intended
    /// to create a more realistic or pleasing representation of audio by
    /// representing sound coming from multiple directons.
    /// 
    /// Single Channel: Also known as Mono. Typically the reproduction of a single
    /// independent audio stream in one device or of the same independent audio stream
    /// in multiple devices.
    /// 
    /// Dual Channel: Two independent Mono channels. May overlap with stereo or may 
    /// be completely independent as in the case of foreign language audio dubbing.
    /// 
    /// Joint Stereo: The joining of multiple channels of audio to create another separate
    /// one, to reduce the size of the file, or to increase the quality.
    /// </summary>
    public enum Channel
    {
        Stereo = 0,
        JointStereo,
        DualChannel,
        SingleChannel,
    }
}
