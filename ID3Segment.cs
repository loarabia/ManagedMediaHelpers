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
using System.IO;

namespace ManagedMediaParsers
{
    /// <summary>
    /// TODO: REWORK TO TAKE INTO ACCOUNT V1 vs V2 ID3 Tags 
    /// </summary>
    public class ID3Segment
    {
        private const int ID3 = 4801587;        // "ID3" This is IDE V2.X
        private const int TAG = 5521735;        // "TAG" This is ID3 V1.X



        private int _majorVersion = 0;
        private int _minorVersion = 0;
        private int _revision = 0;

        private int _totalID3Size = 0;

        private byte[] _id3Segment = null;
        private byte[] _id3Header = null;

        public int MajorVerson
        {
            get
            {
                return _majorVersion;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public ID3Segment(Stream stream)
        {
            long startPosition = stream.Position;
            byte[] id3SegmentIdentifier = new byte[3];

            /*
             * Id3 header is 10 Bytes in 2.4.0, 2.3.0, and 2.2.0
             *  3 bytes:    "ID3"
             *  2 bytes:    version
             *  1 byte:     flags 
             *  4 bytes:    Id3 sizes without header and footer
             *  
             * An extended header could be variable sized
             * For more information on the 2.4 Id3 structure see:
             * http://www.id3.org/id3v2.4.0-structure
             * 
             * Id3 tag in version 1 is 128 bytes at the end of a file.
             *  3 bytes:    "TAG"
             *  30 bytes:   Song title
             *  30 bytes:   Album title
             *  3 bytes:    Year
             *  30 bytes:   comment
             *  1 bytes:    genre
             *  
             * ID3 1.1 makes the following changes to the comment / genre.
             *  28 bytes:   comment
             *  1 byte:     "0"
             *  1 byte:     track
             *  1 byte:     genre
             */
            if (stream.Read(id3SegmentIdentifier, 0, 3) != 3)
            {
                stream.Position = startPosition;
                return;
            }

            // Build the string from the first 3 bytes
            int identifier = 0;
            identifier |= id3SegmentIdentifier[0] << 16;  // I | T byte
            identifier |= id3SegmentIdentifier[1] << 8;   // D | A byte
            identifier |= id3SegmentIdentifier[2];        // 3 | G byte

            // Compare to see if it is "ID3" (ID3 2.X ) or "TAG" (ID3 1.X)
            startPosition = stream.Position;
            if (identifier == ID3)
            {
                _majorVersion = 2;
                _id3Header = new byte[7];
                if (stream.Read(_id3Header, 0, 7) != 7)
                {
                    stream.Position = startPosition;
                }
                // 2 Bytes
                // Version and Revision
                _minorVersion = (int)_id3Header[0];
                _revision = (int)_id3Header[1];

                // 1 Byte
                // Flags ( Footer )
                bool hasFooter = true;
                if ((_id3Header[2] & (1 << 4)) == 0)
                {
                    hasFooter = false;
                }

                // 4 Bytes
                // Size as a 32 bit synchsafe integer
                // See the same link as above about ID3 tags            
                _totalID3Size = BitTools.convertSSIntToInt(
				_id3Header[3],
			       	_id3Header[4],
			       	_id3Header[5],
			       	_id3Header[6]);

                // Add in the size of the Header
                _totalID3Size += 10;

                // Add in the size of the Footer if there is one
                if (hasFooter)
                {
                    _totalID3Size += 10;
                }
                // TEMP Move the stream forward by the proper amount
                stream.Position += _totalID3Size - 10;
            }
            else if (identifier == TAG)
            {
                _majorVersion = 1;
                _id3Header = new byte[125];
                if (stream.Read(_id3Header, 0, 125) != 125)
                {
                    stream.Position = startPosition;
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public int TotalID3Size
        {
            get { return _totalID3Size; }
        }



    }
}
