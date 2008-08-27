/******************************************************************************
 * (c) Copyright Larry Olson.
 * This source is subject to the Microsoft Reciprocal License (Ms-RL)
 * See http://www.microsoft.com/resources/sharedsource/licensingbasics/reciprocallicense.mspx
 * All other rights reserved.
 ******************************************************************************/
using System;
using System.IO;
using System.Diagnostics;

namespace ManagedMediaParsers
{
    /// <summary>
    ///  
    /// </summary>
    //TODO: Anything special about the dot releases of ID3 that should be
    // accounted for?
    public class Id3Segment
    {
        private const int ID3 = 4801587;        // "ID3" This is ID3 V2.X
        private const int TAG = 5521735;        // "TAG" This is ID3 V1.X
        private const int BUFFER_SIZE = 10000;


        byte[] _id3Header;

        /// <summary>
        /// 
        /// </summary>
        public int MajorVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length{ get; private set;}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <remarks>
        /// Id3 header is 10 Bytes in 2.4.0, 2.3.0, and 2.2.0
        ///  3 bytes:    "ID3"
        ///  2 bytes:    version
        ///  1 byte:     flags 
        ///  4 bytes:    Id3 sizes without header and footer
        ///     
        /// An extended header could be variable sized
        /// For more information on the 2.4 Id3 structure see:
        /// http://www.id3.org/id3v2.4.0-structure
        ///     
        /// Id3 tag in version 1 is 128 bytes at the end of a file.
        ///  3 bytes:    "TAG"
        ///  30 bytes:   Song title
        ///  30 bytes:   Album title
        ///  3 bytes:    Year
        ///  30 bytes:   comment
        ///  1 bytes:    genre
        ///       
        /// ID3 1.1 makes the following changes to the comment / genre.
        ///  28 bytes:   comment
        ///  1 byte:     "0"
        ///  1 byte:     track
        ///  1 byte:     genre
        /// </remarks>
        public Id3Segment(Stream fileStream)
        {
            if (fileStream.Position == FindSyncPoint(fileStream))
            {
                ParseId3(fileStream);
            }
            else
            {
                throw new InvalidOperationException("Stream does not have ID3 tags.");
            }
        }

/******************************************************************************
 * FindSyncPoint
 * 
 * Summary:
 *  Searches a stream for the "ID3" or "TAG" that represents the point at which
 *  an ID3 tag begins. 
 *  
 * Input:
 *  The stream to search, typically an audio stream. 
 * 
 * Output:
 *  The position in the stream at which the I or T was found. The stream will
 *  be left pointing to that position. 
 *  
 *  If the stream cannot be seeked or read or the search turns up nothing, this
 *  method returns the start position of the stream.
 *****************************************************************************/
        //TODO: 2 types of parallelism that could be employed here if needed.
        //  1. Call BeginRead to buffer data and while that is buffering search
        //      the current set of data
        //  2. Do a parallel search of the data buffer
        //TODO: Some streams from WebResponses will have a length of:
        //  -1 ( cannot Seek )
        //  0 ( Response stream is still null )
        //  Size of the WebRequest
        //  Size of the Stream
        //TODO: Only parses "ID3" / "TAG". This should be generalizeable.
        private static long FindSyncPoint(Stream fleStream)
        {
            long startPosition = fleStream.Position;
            // Guard 
            if (fleStream.CanSeek == false || fleStream.CanRead == false) { return startPosition; }

            // Linear search
            byte[] data = new byte[BUFFER_SIZE];       // Buffer of data
                                                   

            string partialFind = "";

            // Inner loop variables
            int bytesRead = 0;
            long index;

            for (long streamIndex = 0, cachedStreamLength = fleStream.Length -1;
                streamIndex < cachedStreamLength;
                streamIndex++)
            {
                index = streamIndex % (bytesRead - 1);
                if (index == 0)
                {
                    bytesRead = fleStream.Read(data, 0, data.Length);
                }

                if (partialFind.Length > 0)
                {
                    // To even get here you must have seen an "I" or "ID"
                    partialFind += (char)data[index];

                    // ID + 3
                    if (partialFind == "TAG" || partialFind == "ID3" )
                    {
                        fleStream.Position = fleStream.Position - bytesRead + (index + 1) - partialFind.Length;
                        return fleStream.Position;
                    }
                    // I + D
                    else if (partialFind == "TA" || partialFind == "ID" )
                    {
                        continue; // Go straight to the next for iteration
                    }
                    // EDGE CASE something like IID3 or IDID3 start over
                    else if ('T' == (char)data[index] || 'I' == (char)data[index])
                    {
                        partialFind = "";
                        partialFind += (char)data[index];
                        continue; // Go straight to the next for iteration
                    }
                    // Nothing
                    else
                    {
                        partialFind = "";
                    }
                }
                else if ('T' == (char)data[index] || 'I' == (char)data[index])
                {
                    partialFind += (char)data[index];
                }
            }
            return startPosition;
        }

/******************************************************************************
 * ParseID3
 * 
 * Summary:
 * 
 * Input:
 * 
 *****************************************************************************/
        private void ParseId3(Stream fileStream)
        {
            byte[] id3SegmentIdentifier = new byte[3];
            long startPosition = fileStream.Position;

            if (fileStream.Read(id3SegmentIdentifier, 0, 3) != 3)
            {
                fileStream.Position = startPosition;
                return;
            }

            //// Build the string from the first 3 bytes
            int identifier = 0;
            identifier |= id3SegmentIdentifier[0] << 16;  // I | T byte
            identifier |= id3SegmentIdentifier[1] << 8;   // D | A byte
            identifier |= id3SegmentIdentifier[2];        // 3 | G byte

            //// Compare to see if it is "ID3" (ID3 2.X ) or "TAG" (ID3 1.X)
            startPosition = fileStream.Position;
            if (identifier == ID3)
            {
                MajorVersion = 2;
                _id3Header = new byte[7];
                if (fileStream.Read(_id3Header, 0, 7) != 7)
                {
                    fileStream.Position = startPosition;
                }
                // 2 Bytes
                // Version and Revision
                //_minorVersion = (int)_id3Header[0];
                //_revision = (int)_id3Header[1];

                // 1 Byte
                // Flags ( Footer )
                bool hasFooter = true;
                if ((_id3Header[2] & (1 << 4)) == 0)
                {
                    hasFooter = false;
                }

                // 4 Bytes
                // Size as a 32 bit synchsafe integer
                // See the link in the ID3 segment constructor            
                Length = BitTools.ConvertToSyncSafeInt32( _id3Header,3);

                // Add in the size of the Header
                Length += 10;

                // Add in the size of the Footer if there is one
                if (hasFooter)
                {
                    Length += 10;
                }
                // Due to Reads, s.Position is already 10 forward
                // Length includes the total length so when adjusting the
                // stream forward, remove the 10 that have already been read.
                fileStream.Position += Length - 10;
            }
            else if (identifier == TAG)
            {
                MajorVersion = 1;
                _id3Header = new byte[125];
                if (fileStream.Read(_id3Header, 0, 125) != 125)
                {
                    fileStream.Position = startPosition;
                }
            }
        }
    }
}