//-----------------------------------------------------------------------
// <copyright file="ID3Segment.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
// This source is subject to the Microsoft Reciprocal License (Ms-RL)
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/reciprocallicense.mspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ManagedMediaParsers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    
    /// <summary>
    ///  Foo Bar Baz!
    /// </summary>
    /// TODO: Anything special about the dot releases of ID3 that should be accounted for?
    public class Id3Segment
    {
        /// <summary>
        /// "ID3" This is ID3 V2.X
        /// </summary>
        private const int ID3 = 4801587;

        /// <summary>
        /// "TAG" This is ID3 V1.X
        /// </summary>
        private const int TAG = 5521735;

        /// <summary>
        /// This is a configruation point for the size of the Buffer (which buffer?)
        /// </summary>
        private const int BufferSize = 10000;

        /// <summary>
        /// Foo Bar Baz!
        /// </summary>
        private byte[] id3Header;

        /// <summary>
        /// Initializes a new instance of the Id3Segment class.
        /// </summary>
        /// <param name="fileStream">
        /// The filestreamt that contains the ID3Segement.
        /// </param>
        /// <remarks>
        /// <para>
        /// Id3 header is 10 Bytes in 2.4.0, 2.3.0, and 2.2.0
        ///  3 bytes:    "ID3"
        ///  2 bytes:    version
        ///  1 byte:     flags 
        ///  4 bytes:    Id3 sizes without header and footer
        /// </para>
        /// <para>
        /// An extended header could be variable sized
        /// For more information on the 2.4 Id3 structure see:
        /// http://www.id3.org/id3v2.4.0-structure
        /// </para>
        /// <para>
        /// Id3 tag in version 1 is 128 bytes at the end of a file.
        ///  3 bytes:    "TAG"
        ///  30 bytes:   Song title
        ///  30 bytes:   Album title
        ///  3 bytes:    Year
        ///  30 bytes:   comment
        ///  1 bytes:    genre
        /// </para>       
        /// <para>
        /// ID3 1.1 makes the following changes to the comment / genre.
        ///  28 bytes:   comment
        ///  1 byte:     "0"
        ///  1 byte:     track
        ///  1 byte:     genre
        /// </para>
        /// </remarks>
        public Id3Segment(Stream fileStream)
        {
            if (fileStream.Position == FindSyncPoint(fileStream))
            {
                this.ParseId3(fileStream);
            }
            else
            {
                throw new InvalidOperationException("Stream does not have ID3 tags.");
            }
        }

        /// <summary>
        /// Gets Foo Bar Baz!
        /// </summary>
        public int MajorVersion { get; private set; }

        /// <summary>
        /// Gets Foo Bar Baz!
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        ///  Searches a stream for the "ID3" or "TAG" that represents the point at which 
        ///  an ID3 tag begins. 
        /// </summary>
        /// <param name="fleStream">The stream to search, typically an audio stream.</param>
        /// <returns>
        /// The position in the stream at which the I or T was found. The stream will be left pointing to that position. 
        /// If the stream cannot be seeked or read or the search turns up nothing, this method returns the start position of the stream.
        /// </returns>
        private static long FindSyncPoint(Stream fleStream)
        {
            /*
             * TODO: 2 types of parallelism that could be employed here if needed.
             *  1. Call BeginRead to buffer data and while that is buffering search the current set of data
             *  2. Do a parallel search of the data buffer
             * TODO: Some streams from WebResponses will have a length of:
             *  -1 ( cannot Seek )
             *  0 ( Response stream is still null )
             *  Size of the WebRequest
             *  Size of the Stream
             *  TODO: Only parses "ID3" / "TAG". This should be generalizeable.
             */

            long startPosition = fleStream.Position;

            // Guard 
            if (fleStream.CanSeek == false || fleStream.CanRead == false)
            {
                return startPosition; 
            }

            // Linear search
            byte[] data = new byte[BufferSize];       // Buffer of data
                                                   
            string partialFind = string.Empty;

            // Inner loop variables
            int bytesRead = 0;
            long index;

            for (long streamIndex = 0, cachedStreamLength = fleStream.Length - 1;
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
                    if (partialFind == "TAG" || partialFind == "ID3")
                    {
                        fleStream.Position = fleStream.Position - bytesRead + (index + 1) - partialFind.Length;
                        return fleStream.Position;
                    }
                    else if (partialFind == "TA" || partialFind == "ID")
                    {
                        // I + D
                        // Go straight to the next for iteration
                        continue;
                    }
                    else if ('T' == (char)data[index] || 'I' == (char)data[index])
                    {
                        // EDGE CASE something like IID3 or IDID3 start over
                        partialFind = string.Empty;
                        partialFind += (char)data[index];

                        // Go straight to the next for iteration
                        continue;
                    }
                    else
                    {
                        // Nothing
                        partialFind = string.Empty;
                    }
                }
                else if ('T' == (char)data[index] || 'I' == (char)data[index])
                {
                    partialFind += (char)data[index];
                }
            }

            return startPosition;
        }

        /// <summary>
        /// Parses out the ID3 structure from the filestream.
        /// </summary>
        /// <param name="fileStream">The filestream which contains the ID3 structure</param>
        private void ParseId3(Stream fileStream)
        {
            byte[] id3SegmentIdentifier = new byte[3];
            long startPosition = fileStream.Position;

            if (fileStream.Read(id3SegmentIdentifier, 0, 3) != 3)
            {
                fileStream.Position = startPosition;
                return;
            }

            // Build the string from the first 3 bytes
            int identifier = 0;
            identifier |= id3SegmentIdentifier[0] << 16;  // I | T byte
            identifier |= id3SegmentIdentifier[1] << 8;   // D | A byte
            identifier |= id3SegmentIdentifier[2];        // 3 | G byte

            // Compare to see if it is "ID3" (ID3 2.X ) or "TAG" (ID3 1.X)
            startPosition = fileStream.Position;
            if (identifier == ID3)
            {
                this.MajorVersion = 2;
                this.id3Header = new byte[7];
                if (fileStream.Read(this.id3Header, 0, 7) != 7)
                {
                    fileStream.Position = startPosition;
                }

                // 2 Bytes
                // Version and Revision
                ////_minorVersion = (int)this.id3Header[0];
                ////_revision = (int)this.id3Header[1];

                // 1 Byte
                // Flags ( Footer )
                bool hasFooter = true;
                if ((this.id3Header[2] & (1 << 4)) == 0)
                {
                    hasFooter = false;
                }

                // 4 Bytes
                // Size as a 32 bit synchsafe integer
                // See the link in the ID3 segment constructor            
                this.Length = BitTools.ConvertToSyncSafeInt32(this.id3Header, 3);

                // Add in the size of the Header
                this.Length += 10;

                // Add in the size of the Footer if there is one
                if (hasFooter)
                {
                    this.Length += 10;
                }

                // Due to Reads, s.Position is already 10 forward
                // Length includes the total length so when adjusting the
                // stream forward, remove the 10 that have already been read.
                fileStream.Position += this.Length - 10;
            }
            else if (identifier == TAG)
            {
                this.MajorVersion = 1;
                this.id3Header = new byte[125];
                if (fileStream.Read(this.id3Header, 0, 125) != 125)
                {
                    fileStream.Position = startPosition;
                }
            }
        }
    }
}