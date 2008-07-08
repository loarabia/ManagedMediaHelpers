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

namespace ManagedMediaParsers
{
    /// <summary>
    /// 
    /// </summary>
    public class WAVEFORMATEX
    {
        public short FormatTag { get; set; }
        public short Channels { get; set; }
        public int SamplesPerSec { get; set; }
        public int AvgBytesPerSec { get; set; }
        public short BlockAlign { get; set; }
        public short BitsPerSample { get; set; }
        public short Size { get; set; }

        public override string ToString()
        {
            char[] rawData = new char[18];
            BitConverter.GetBytes(FormatTag).CopyTo(rawData, 0);
            BitConverter.GetBytes(Channels).CopyTo(rawData, 2);
            BitConverter.GetBytes(SamplesPerSec).CopyTo(rawData, 4);
            BitConverter.GetBytes(AvgBytesPerSec).CopyTo(rawData, 8);
            BitConverter.GetBytes(BlockAlign).CopyTo(rawData, 12);
            BitConverter.GetBytes(BitsPerSample).CopyTo(rawData, 14);
            BitConverter.GetBytes(Size).CopyTo(rawData, 16);
            return new string(rawData);
        }
    }

    /// <summary>
    /// 
    /// </summary> 
    public class MPEGLAYER3WAVEFORMAT
    {
        public WAVEFORMATEX wfx { get; set; }
        public short ID { get; set; }
        public int Flags { get; set; }
        public short BlockSize { get; set; }
        public short FramesPerBlock { get; set; }
        public short CodecDelay { get; set; }

        public override string ToString()
        {
            char[] rawData = new char[12];
            BitConverter.GetBytes(ID).CopyTo(rawData, 0);
            BitConverter.GetBytes(Flags).CopyTo(rawData, 2);
            BitConverter.GetBytes(BlockSize).CopyTo(rawData, 6);
            BitConverter.GetBytes(FramesPerBlock).CopyTo(rawData, 8);
            BitConverter.GetBytes(CodecDelay).CopyTo(rawData, 10);
            return wfx.ToString() + new string(rawData);
        }
    }
}
