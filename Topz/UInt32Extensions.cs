using System;

namespace Topz
{
    /// <summary>
    /// Provides extensions for the <see cref="UInt32"/> struct.
    /// </summary>
    public static class UInt32Extensions
    {
        /// <summary>
        /// Sets a bit to 1 or 0. 
        /// </summary>
        /// <param name="number">The number to operate on.</param>
        /// <param name="index">The index of the bit to manipulate starting from the right.</param>
        /// <param name="value">The value the bit is set to.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than 31.
        /// </exception>
        /// <returns>The new value.</returns>
        public static uint SetBit(this uint number, byte index, bool value)
        {
            if (index > 31)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (value)
                return number | (1u << index);
            
            return number & ~(1u << index);
        }

        /// <summary>
        /// Converts an integer to binary.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>The binary value.</returns>
        public static string ToBinary(this uint value)
        {
            var text = Convert.ToString(value, 2);
            text = text.PadLeft(32, '0');

            for (int i = 4; i <= text.Length; i += 4)
                text = text.Insert(i++, " ");

            return text.Substring(0, text.Length - 1);
        }

        /// <summary>
        /// Converts an integer to binary.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>The binary value.</returns>
        public static string ToBinary(this int value)
        {
            var text = Convert.ToString(value, 2);
            text = text.PadLeft(32, '0');

            for (int i = 4; i <= text.Length; i += 4)
                text = text.Insert(i++, " ");

            return text.Substring(0, text.Length - 1);
        }
    }
}