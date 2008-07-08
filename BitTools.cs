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
    public class BitTools
    {
        private const int SYNC_SAFE_MASK = 127; // 0111 1111

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="startingBit"></param>
        /// <param name="maskSize"></param>
        /// <returns></returns>
        public static int MaskBits(byte[] header, int startingBit, int maskSize)
        {
            // Clear out numbers which are too small
            if (header.Length <= 0 || startingBit < 0 || maskSize <= 0)
            {
                return -1;
            }

            // Clear out numbers where you are masking outside of the valid range.
            if ((startingBit + maskSize) > header.Length * 8)
            {
                return -1;
            }

            // Clear out masks which are larger than the number of bits in an int
            if (maskSize > sizeof(int) * 8)
            {
                return -1;
            }

            // Figure out what byte the starting bit is in
            int startByteIndex = startingBit / 8; // Integer divide by 8

            // Figure what byte the ending bit is in
            int endByteIndex = (startingBit + maskSize - 1) / 8; // Integer divide by 8

            // initialize the mask
            int mask = 0;

            // Build an initial mask with the proper number of bits set to 1
            int i = 0;
            while (i < maskSize)
            {
                mask |= (1 << i);
                i++;
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
            for (int byteIndex = startByteIndex; byteIndex <= endByteIndex; byteIndex++)
            {
                temp = header[byteIndex];

                // Shift it to the right byte position
                temp = temp << ((byteIndex) * 8);
                headerValue = headerValue | temp;
            }

            // shift the bits to the right to make an int
            headerValue = headerValue >> startingBit;

            // mask out the appropriate bits
            headerValue = headerValue & mask;

            return (int)headerValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bigEnd"></param>
        /// <param name="bigMidByte"></param>
        /// <param name="littleMidByte"></param>
        /// <param name="littleEnd"></param>
        /// <returns></returns>
        public static int convertSSIntToInt(
            int bigEnd,
            int bigMidByte,
            int littleMidByte,
            int littleEnd)
        {
            int number = 0;
            number = number | ((bigEnd & SYNC_SAFE_MASK) << 23);
            number = number | ((bigMidByte & SYNC_SAFE_MASK) << 15);
            number = number | ((littleMidByte & SYNC_SAFE_MASK) << 7);
            number = number | littleEnd;

            return number;
        }
    }
}
