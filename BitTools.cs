//-----------------------------------------------------------------------
// <copyright file="BitTools.cs" company="Larry Olson">
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

    /// <summary>
    /// Helper methods for manipulating values at the byte and binary level.
    /// </summary>
    public static class BitTools
    {
        /// <summary>
        /// Defined by ID3v2 spec as 4 bytes
        /// </summary>
        private const int SyncSafeIntegerSize = 4;

        /// <summary>
        /// 1 Byte is 8 bits 
        /// </summary>
        private const int ByteSize = 8;

        /// <summary>
        /// Masks out up to an integer sized (4 bytes) set of bits from an
        /// array of bytes.
        /// </summary>
        /// <param name="data">An array of data in Little Endian Order</param>
        /// <param name="firstBit">The bit index of the first bit</param>
        /// <param name="maskSize">The length of the mask in bits</param>
        /// <returns>An integer of the bits masked out</returns>
        public static int MaskBits(byte[] data, int firstBit, int maskSize)
        {
            // Guard against null data
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // Clear out numbers which are too small
            if (data.Length <= 0 || firstBit < 0 || maskSize <= 0)
            {
                throw new ArgumentException("data,firstBit,or MaskSize are too small");
            }

            // Clear out numbers where you are masking outside of the valid
            // range
            if ((firstBit + maskSize) > data.Length * ByteSize) 
            {
                throw new ArgumentException("Attempting to mask outside of the data array");
            }

            // Clear out masks which are larger than the number of bits in an
            // int
            if (maskSize > sizeof(int) * ByteSize) 
            {
                throw new ArgumentException("maskSize is larger than an integer");
            }

            // Figure out what byte the starting bit is in
            int startByteIndex = firstBit / ByteSize; // Int div

            // Figure what byte the ending bit is in
            int endByteIndex = (firstBit + maskSize - 1) / ByteSize; // Int div

            // initialize the mask
            int mask = 0;

            // Build an initial mask with the proper number of bits set to 1
            for (int i = 0; i < maskSize; i++)
            {
                mask |= (1 << i);
            }

            // initialize the return value
            long headerValue = 0;

            // initialize the bytes to be masked
            /*
             * The desired bits could be spread across 5 bytes
             * but they probably will be spread over fewer bytes
             */
            long temp;
            int shiftAmount;
            for (int bi = startByteIndex; bi <= endByteIndex; bi++)
            {
                temp = data[bi];
                                
                shiftAmount = (endByteIndex - bi) * ByteSize;
                temp = temp << shiftAmount;
                
                headerValue = headerValue | temp;
            }

            // shift the bits to the right to make an int
            headerValue = headerValue >> firstBit % 8;

            // mask out the appropriate bits
            headerValue = headerValue & mask;

            return (int)headerValue;
        }

        /// <summary>
        /// Converts a Syncronization Safe integer from the ID3v2 spec into a
        /// standard integer.
        /// </summary>
        /// <param name="syncSafeData">
        /// An array of bytes containing raw data in Syncronization Safe format
        /// as defined in the ID3v2 spec. This means that it is a 4 byte
        /// integer where the leading  bit of each byte is a 0. 
        /// For Example:
        /// 01111111 01111111 01111111 01111111
        /// Output would be:
        /// 00001111 11111111 11111111 11111111
        /// Assumes syncSafeData array is in Big Endiah Order.
        /// </param>
        /// <param name="startIndex">
        /// Where in the array of bytes, the syncsafe data starts. Note that
        /// data's size is assumed to be 4 bytes in length.
        /// </param>
        /// <returns>
        /// A standard integer. Note that this integer can only have a data
        /// resolution of 28 bits (max value of this could only be 2^28 -1).
        /// </returns>
        public static int ConvertSyncSafeToInt32(
            byte[] syncSafeData,
            short startIndex)
        {
            int integer = 0;
            int syncSafeByte = 0; // Store byte in an int to enable shifting
            int shiftAmount = 0;

            // Guard
            if (syncSafeData == null)
            {
                throw new ArgumentNullException("syncSafeData");
            }

            // Shifts the first three bytes left and copies them into the int
            // Stop shifting before you hit the last byte. The last byte is
            // already where it needs to be
            int i;
            for (i = 0; i < SyncSafeIntegerSize - 1; i++)
            {
                syncSafeByte = syncSafeData[startIndex + i];
                shiftAmount = (ByteSize - 1) * (SyncSafeIntegerSize - 1 - i);
                integer |= syncSafeByte << shiftAmount;
            }

            // Copy the unshifted fourth bit into the int
            syncSafeByte = syncSafeData[startIndex + i];
            integer |= syncSafeByte;

            return integer;
        }

        /// <summary>
        /// Searches a byte array for a pattern of bits.
        /// </summary>
        /// <param name="data">
        /// The array of bytes to search for the pattern within.
        /// </param>
        /// <param name="pattern">
        /// The pattern of bytes to match with undesired bits zeroed out.
        /// </param>
        /// <param name="mask">
        /// A mask to zero out bits that aren't part of the pattern.
        /// </param>
        /// <returns>
        /// Returns the location of the first byte in the pattern or -1 if
        /// nothing was found or there was an error.
        /// </returns>
        public static int FindBitPattern(byte[] data, byte[] pattern, byte[] mask)
        {
            // GUARD
            if (pattern.Length < 0 || data.Length < 0 || data.Length < pattern.Length || mask.Length != pattern.Length)
            {
                return -1;
            }

            int di = 0; // data index
            int pati = 0; // pattern index

            while (di < data.Length)
            {
                if (pati == pattern.Length)
                {
                    return di - pattern.Length;
                }
                else if (pattern[pati] == (data[di] & mask[pati]))
                {
                    pati++;
                }
                else if (pattern[pati] != (data[di] & mask[pati]))
                {
                    pati = 0;
                }
                else
                {
                    Debug.Assert(false, "All possible states should have already been covered.");
                }

                di++;
            }

            return -1;
        }

        /// <summary>
        /// Searches a byte array for a pattern of bytes.
        /// </summary>
        /// <param name="data">
        /// The array of bytes to search for the pattern within.
        /// </param>
        /// <param name="pattern">
        /// The pattern of bytes to match.
        /// </param>
        /// <returns>
        /// Returns the location of the first byte in the pattern or -1 if
        /// nothing was found or there was an error.
        /// </returns>
        public static int FindBytePattern(byte[] data, byte[] pattern)
        {
            byte[] mask = new byte[pattern.Length];
            for (int i = 0; i < pattern.Length; i++)
            {
                mask[i] = byte.MaxValue; // 1111 1111
            }

            return FindBitPattern(data, pattern, mask);
        }
    }
}
