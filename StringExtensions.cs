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
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bigEndianString"></param>
        /// <returns></returns>
        public static string ToLittleEndian(this string bigEndianString)
        {
            // Guard
            if (bigEndianString == null) { return ""; }

            char[] bigEndianChars = bigEndianString.ToCharArray();

            // Guard
            if (bigEndianChars.Length % 2 != 0) { return ""; }

            int i, ai, bi, ci, di;
            char a, b, c, d;

            for (i = 0; i < bigEndianChars.Length / 2; i += 2)
            {
                // front byte
                ai = i;
                bi = i + 1;

                // back byte
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
