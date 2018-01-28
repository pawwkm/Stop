using System;

namespace Topz
{
    /// <summary>
    /// Provides extensions for the <see cref="UInt32"/> struct.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Sets a bit to 1 or 0. 
        /// </summary>
        /// <param name="number">The number to operate on.</param>
        /// <param name="index">The index of the bit to manipulate starting from the right.</param>
        /// <param name="value">The value the bit is set to.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than 7.
        /// </exception>
        /// <returns>The new value.</returns>
        public static byte SetBit(this byte number, byte index, bool value)
        {
            if (index > 7)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (value)
                return (byte)(number | (1 << index));

            return (byte)(number & ~(1 << index));
        }
    }
}