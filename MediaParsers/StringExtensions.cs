//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
// This source is subject to the Microsoft Public License (Ms-PL)
// See http://code.msdn.microsoft.com/ManagedMediaHelpers/Project/License.aspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// Supressing Code Analysis rule(s)
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace",
    Target = "ExtensionMethods",
    Justification = "This is appropriate as a separate namespace because it logically is separate from the ManagedMediaParsers namespace.")]

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes",
    Scope = "member",
    Target = "ExtensionMethods.StringExtensions.#ToLittleEndian(System.String)",
    Justification = "This is appropriate to make this method look like a first class member of string.")]

namespace ExtensionMethods
{
    using System;

    /// <summary>
    /// Extensions for the standard string class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// <para>
        /// Converts a string of characters from Big Endian byte order to
        /// Little Endian byte order.
        /// </para>
        /// <para>
        /// Assumptions this makes about the string. Every two characters
        /// make up the smallest data unit (analogous to byte). The entire
        /// string is the size of the systems natural unit of data (analogous
        /// to a word).
        /// </para>
        /// </summary>
        /// <param name="value">
        /// A string in Big Endian Byte order.
        /// </param>
        /// <returns>
        /// A string in Little Endian Byte order.
        /// </returns>
        /// <remarks>
        /// This function was designed to take in a Big Endian string of
        /// hexadecimal digits.
        /// <example>
        /// input:
        ///     DEADBEEF
        /// output:
        ///     EFBEADDE
        /// </example>
        /// </remarks>
        public static string ToLittleEndian(this string value)
        {
            // Guard
            if (value == null)
            {
                throw new NullReferenceException();
            }

            char[] bigEndianChars = value.ToCharArray();

            // Guard
            if (bigEndianChars.Length % 2 != 0)
            {
                return string.Empty;
            }

            int i, ai, bi, ci, di;
            char a, b, c, d;

            for (i = 0; i < bigEndianChars.Length / 2; i += 2) 
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
