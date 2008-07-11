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

namespace ExtensionMethods
{
    /// <summary>
    /// Extensions for the standard string class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string of characters from Big Endian byte order to
        /// Little Endian byte order.
        /// 
        /// Assumptions this makes about the string. Every two characters
        /// make up the smallest data unit (analogous to byte). The entire
        /// string is the size of the systems natural unit of data (analogous
        /// to a word).
        /// </summary>
        /// <param name="bigEndianString">
        /// A string in Big Endian Byte order.
        /// </param>
        /// <returns>
        /// A string in Little Endian Byte order.
        /// </returns>
        /// <remarks>
        /// This function was designed to take in a Big Endian string of
        /// hexadecimal digits.
        /// 
        /// Example input:
        ///     DEADBEEF
        /// Example output:
        ///     EFBEADDE
        /// </remarks>
        // TODO: Not quite the right way to do this but good enough for now
        public static string ToLittleEndian(this string bigEndianString)
        {
            // Guard
            if (bigEndianString == null) { return ""; }

            char[] bigEndianChars = bigEndianString.ToCharArray();

            // Guard
            if (bigEndianChars.Length % 2 != 0) { return ""; }

            int i, ai, bi, ci, di;
            char a, b, c, d;

            for (i = 0; i < bigEndianChars.Length / 2; i += 2) //TODO: bug here
            {
                // front byte ( in hex )
                ai = i;
                bi = i + 1;

                // back byte ( in hex )
                ci = bigEndianChars.Length - 2 - i;
                di = bigEndianChars.Length - 1 - i;

                a = bigEndianChars[ai];
                b = bigEndianChars[bi];
                c = bigEndianChars[ci];
                d = bigEndianChars[di];

                bigEndianChars[ci] = a;
                bigEndianChars[di] = b;
                bigEndianChars[ai] = c;
                bigEndianChars[bi] = d;
            }

            return new string(bigEndianChars);
        }
    }
}
