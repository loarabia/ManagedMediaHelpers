/******************************************************************************
 * (c) Copyright Microsoft Corporation.
 * This source is subject to the Microsoft Reciprocal License (Ms-RL)
 * See http://www.microsoft.com/resources/sharedsource/licensingbasics/reciprocallicense.mspx
 * All other rights reserved.
 ******************************************************************************/
using System;


namespace ManagedMediaParsers
{
    /// <summary>
    /// Helper methods for manipulating values at the byte and binary level.
    /// </summary>
    public class BitTools
    {
        // defined by ID3v2 spec as 4 bytes
        private const int SYNC_SAFE_INT_SIZE = 4;

        // 1 Byte is 8 bits
        private const int BYTE_SIZE = 8; 

        /// <summary>
        /// Masks out a set of bits from an array of bytes.
        /// </summary>
        /// <param name="header">An array of data</param>
        /// <param name="startingBit">The bit index of the first bit</param>
        /// <param name="maskSize">The length of the mask in bits</param>
        /// <returns></returns>
        public static int MaskBits(byte[] array, int firstBit, int maskSize)
        {
            // Clear out numbers which are too small
            if (array.Length <= 0 || firstBit < 0 || maskSize <= 0){return -1;}

            // Clear out numbers where you are masking outside of the valid
            // range
            if ((firstBit + maskSize) > array.Length * BYTE_SIZE) {return -1;}

            // Clear out masks which are larger than the number of bits in an
            // int
            if (maskSize > sizeof(int) * BYTE_SIZE) { return -1; }

            // Figure out what byte the starting bit is in
            int startByteIndex = firstBit / BYTE_SIZE; // Int div

            // Figure what byte the ending bit is in
            int endByteIndex = (firstBit + maskSize - 1) / BYTE_SIZE; // Int div

            // initialize the mask
            int mask = 0;

            // Build an initial mask with the proper number of bits set to 1
            for (int i = 0; i < maskSize; i++)
            {
                mask |= (1 << i);
            }

            // initialize the return value
            Int64 headerValue = 0;

            // initialize the bytes to be masked
            /*
             * Keep in mind that the bits we want could actually be spread
             * across 5 bytes but they probably will be spread over a lesser
             * number of bytes
             */
            Int64 temp;
            for (int bi = startByteIndex; bi <= endByteIndex; bi++)
            {
                temp = array[bi];

                // Shift it to the right byte position
                temp = temp << ((bi) * BYTE_SIZE);
                headerValue = headerValue | temp;
            }

            // shift the bits to the right to make an int
            headerValue = headerValue >> firstBit;

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
        /// </param>
        /// <param name="syncSafeDataStart">
        /// Where in the array of bytes, the syncsafe data starts. Note that
        /// data's size is assumed to be 4 bytes in length.
        /// </param>
        /// <returns>
        /// A standard integer. Note that this integer can only have a data
        /// resolution of 28 bits (max value of this could only be 2^28 -1).
        /// </returns>
        public static int convertToSyncSafeInt( 
            byte[] syncSafeData,
            short startIndex )
        {
            int integer = 0;
            int syncSafeByte = 0; // Store byte in an int to enable shifting
            int shiftAmount = 0; 

            // Stop shifting before you hit the last byte. The last byte is
            // already where it needs to be
            // Shifts the first three bytes left and copies them into the int
            int i;
            for(i = 0; i < SYNC_SAFE_INT_SIZE - 1; i++)
            {
                syncSafeByte = syncSafeData[startIndex + i];
                shiftAmount = BYTE_SIZE * (SYNC_SAFE_INT_SIZE - 1 - i) - 1;
                integer |= syncSafeByte << (int)shiftAmount;
            }

            // Copy the unshifted fourth bit into the int
            syncSafeByte = syncSafeData[startIndex + i];
            integer |= syncSafeByte;

            return integer;
        }
    }
}
