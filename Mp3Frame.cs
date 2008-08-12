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
    public class Mp3Frame
    {
        private const int SYNC_VALUE = 2047;    // Frame Sync is 12 1s
        private const int FRAME_HEADER_SIZE = 4;// MP3 Headers are 4 Bytes long
        private byte[] _mp3FrameHeader = null;

        private int? _version = null;
        private int? _layer = null;
        private int? _bitrateIndex = null;
        private int? _samplingRateIndex = null;
        private int? _padding = null;
        private bool? _protected = null;
        private Channel? _channels = null;

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

        /// <summary>
        /// 
        /// </summary>
        public int Version
        {
            get
            {
                if (_version != null)
                {
                    return (int)_version;
                }
                else if (_mp3FrameHeader != null)
                {
                    _version = ParseVersion(_mp3FrameHeader);
                    return (int)_version;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Layer
        {
            get
            {
                if (_layer != null)
                {
                    return (int)_layer;
                }
                else if (_mp3FrameHeader != null)
                {
                    _layer = ParseLayer(_mp3FrameHeader);
                    return (int)_layer;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BitrateIndex
        {
            get
            {
                if (_bitrateIndex != null)
                {
                    return (int)_bitrateIndex;
                }
                else if (_mp3FrameHeader != null)
                {
                    _bitrateIndex = BitTools.MaskBits(_mp3FrameHeader, 12, 4);
                    return (int)_bitrateIndex;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SamplingRateIndex
        {
            get
            {
                if (_samplingRateIndex != null)
                {
                    return (int)_samplingRateIndex;
                }
                else if (_mp3FrameHeader != null)
                {
                    _samplingRateIndex 
			    = BitTools.MaskBits(_mp3FrameHeader,
				    10,
				    2);
                    return (int)_samplingRateIndex;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PaddingBit
        {
            get
            {
                if (_padding != null)
                {
                    return (int)_padding;
                }
                else if (_mp3FrameHeader != null)
                {
                    _padding = BitTools.MaskBits(_mp3FrameHeader, 9, 1);
                    return (int)_padding;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsProtected
        {
            get
            {
                if (_protected != null)
                {
                    return (bool)_protected;
                }
                else if (_mp3FrameHeader != null)
                {
                    _protected 
			    = BitTools.MaskBits(_mp3FrameHeader, 16, 1) == 1 ?
			    true :
			    false;
                    return (bool)_protected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="layer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        public Channel Channels
        {
            get
            {
                if (_channels != null)
                {
                    return (Channel)_channels;
                }
                else if (_mp3FrameHeader != null)
                {
                    _channels = ParseChannel(_mp3FrameHeader);
                    return (Channel)_channels;
                }
                else
                {
                    return Channel.SingleChannel;
                }
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public int FrameSize
        {
            get
            {
                switch (Layer)
                {
                    case 1:
                        return (12 * BitRate / SamplingRate + PaddingBit) 
				* 4 - _mp3FrameHeader.Length;
                    case 2:
                    case 3:
                        return 144 * BitRate / SamplingRate 
				+ PaddingBit - _mp3FrameHeader.Length;
                    default:
                        return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "";
            s += "FrameSize\t" + FrameSize + "\n";
            s += "BitRate\t" + BitRate + "\n";
            s += "SamplingRate" + SamplingRate + "\n";
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public Mp3Frame(Stream stream)
        {
            long startPostion = stream.Position;
            _mp3FrameHeader = new byte[FRAME_HEADER_SIZE];

            // Guard against a read error
            if (stream.Read(_mp3FrameHeader, 0, FRAME_HEADER_SIZE)
			    != FRAME_HEADER_SIZE)
            {
                goto cleanup;
            }

            Array.Reverse(_mp3FrameHeader);

            int syncValue = BitTools.MaskBits(_mp3FrameHeader, 21, 11);
            if (!(syncValue == SYNC_VALUE))
            {
                goto cleanup;
            }

            return;
        cleanup:
            stream.Position = startPostion;
            _mp3FrameHeader = null;
            return;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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
    /// 
    /// </summary>
    public enum Channel
    {
        Stereo = 0,
        JointStereo,
        DualChannel,
        SingleChannel,
    }
}
