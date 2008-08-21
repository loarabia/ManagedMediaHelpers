/******************************************************************************
 * (c) Copyright Microsoft Corporation.
 * This source is subject to the Microsoft Reciprocal License (Ms-RL)
 * See http://www.microsoft.com/resources/sharedsource/licensingbasics/reciprocallicense.mspx
 * All other rights reserved.
 ******************************************************************************/
using System;
using ExtensionMethods;
using System.Globalization;


namespace ManagedMediaParsers
{
    /// <summary>
    /// A managed representation of the multimedia WAVEFORMATEX structure
    /// declared in mmreg.h.
    /// </summary>
    /// <remarks>
    /// This was designed for usage in an environment where PInvokes are not
    /// allowed.
    /// </remarks>
    // TODO: struct might be more efficient but these are made so seldom, I
    // doubt it would even be noticeable.
    public class WaveFormatExtensible
    {
        /// <summary>
        /// The audio format type. A complete list of format tags can be
        /// found in the Mmreg.h header file.
        /// </summary>
        /// <remarks>
        /// Silverlight 2 supports:
        /// WMA 7,8,9
        /// WMA 10 Pro
        /// Mp3
        /// 
        /// WAVE_FORMAT_MPEGLAYER3 = 0x0055
        /// </remarks>
        // TODO: Gather the other FormatTag numbers for WMA 7 - 10 and add them
        // to the comments
        public short FormatTag { get; set; }

        /// <summary>
        /// Number of channels in the data. 
        /// Mono            1
        /// Stereo          2
        /// Dual            2 (2 Mono channels)
        /// </summary>
        /// <remarks>
        /// Silverlight 2 only supports stereo output and folds down higher
        /// numbers of channels to stereo.
        /// </remarks>
        public short Channels { get; set; }

        /// <summary>
        /// Sampling rate in hertz (samples per second)
        /// </summary>
        public int SamplesPerSec { get; set; }

        /// <summary>
        /// Average data-transfer rate, in bytes per second, for the format.
        /// </summary>
        public int AverageBytesPerSecond { get; set; }

        /// <summary>
        /// Minimum size of a unit of data for the given format in Bytes.
        /// </summary>
        public short BlockAlign { get; set; }

        /// <summary>
        /// The number of bits in a single sample of the format's data.
        /// </summary>
        public short BitsPerSample { get; set; }

        /// <summary>
        /// The size in bytes of any extra format data added to the end of the
        /// WAVEFORMATEX structure.
        /// </summary>
        public short Size { get; set; }

        /// <summary>
        /// Returns a string representing the structure in little-endian 
        /// hexadecimal format.
        /// </summary>
        /// <remarks>
        /// The string generated here is intended to be passed as 
        /// CodecPrivateData for Silverlight 2's MediaStreamSource
        /// </remarks>
        /// <returns>
        /// A string representing the structure in little-endia hexadecimal
        /// format.
        /// </returns>
        public string ToHexString()
        {
            string s = string.Format(CultureInfo.InvariantCulture, "{0:X4}", FormatTag).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", Channels).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X8}", SamplesPerSec).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X8}", AverageBytesPerSecond).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", BlockAlign).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", BitsPerSample).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", Size).ToLittleEndian();
            return s;
        }

        /// <summary>
        /// Returns a string representing all of the fields in the object.
        /// </summary>
        /// <returns>
        /// A string representing all of the fields in the object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "WAVEFORMATEX FormatTag: {0}, Channels: {1},"
                + "SamplesPerSec: {2}, AvgBytesPerSec: {3}, BlockAlign: {4}, "
                + "BitsPerSample: {5}, Size: {6} ",
                FormatTag, Channels,
                SamplesPerSec, AverageBytesPerSecond, BlockAlign,
                BitsPerSample, Size);
        }
    }

    /// <summary>
    /// A managed representation of the multimedia MPEGLAYER3WAVEFORMATEX 
    /// structure declared in mmreg.h.
    /// </summary>
    /// <remarks>
    /// This was designed for usage in an environment where PInvokes are not
    /// allowed.
    /// </remarks>
    // TODO: struct might be more efficient but these are instantiated so
    // seldom, I doubt it would even be noticeable.
    public class MpegLayer3WaveFormat
    {
        /// <summary>
        /// The core WAVEFORMATEX strucutre representing the Mp3 audio data's
        /// core attributes. 
        /// </summary>
        /// <remarks>
        /// wfx.FormatTag must be WAVE_FORMAT_MPEGLAYER3 = 0x0055 = (85)
        /// wfx.Size must be >= 12
        /// </remarks>
        public WaveFormatExtensible WaveFormatExtensible { get; set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Set this to 
        /// MPEGLAYER3_ID_MPEG = 1
        /// </remarks>
        public short Id { get; set; }

        /// <summary>
        /// Set to determine if padding is needed to adjust the average bitrate
        /// to meet the sampling rate.
        /// 0 = adjust as needed
        /// 1 = always pad
        /// 2 = never pad
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Block Size in bytes. For MP3 audio this is
        /// 144 * bitrate / samplingRate + padding
        /// </summary>
        public short BlockSize { get; set; }

        /// <summary>
        /// Number of frames per block.
        /// </summary>
        //TODO: Always 1?
        public short FramesPerBlock { get; set; }

        /// <summary>
        /// Encoder delay in samples.
        /// </summary>
        public short CodecDelay { get; set; }

        /// <summary>
        /// Returns a string representing the structure in little-endian 
        /// hexadecimal format.
        /// </summary>
        /// <remarks>
        /// The string generated here is intended to be passed as 
        /// CodecPrivateData for Silverlight 2's MediaStreamSource
        /// </remarks>
        /// <returns>
        /// A string representing the structure in little-endia hexadecimal
        /// format.
        /// </returns>
        public string ToHexString()
        {
            string s = WaveFormatExtensible.ToHexString();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", Id).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X8}", Flags).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", BlockSize).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", FramesPerBlock).ToLittleEndian();
            s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", CodecDelay).ToLittleEndian();
            return s;
        }

        /// <summary>
        /// Returns a string representing all of the fields in the object.
        /// </summary>
        /// <returns>
        /// A string representing all of the fields in the object.
        /// </returns>
        public override string ToString()
        {
            return "MPEGLAYER3 "
                + WaveFormatExtensible.ToString()
                + string.Format(CultureInfo.InvariantCulture, 
                    "ID: {0}, Flags: {1}, BlockSize: {2}, "
                    + "FramesPerBlock {3}, CodecDelay {4}",
                    Id, Flags, BlockSize,
                    FramesPerBlock, CodecDelay);
        }
    }
}
