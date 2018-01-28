using System;
using System.IO;

namespace Topz.FileFormats
{
    /// <summary>
    /// Provides extension methods for the <see cref="BinaryWriter"/> class.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes a 2 byte unsigned integer to the current stream encoded in big 
        /// endian and advances the stream position by 2 bytes.
        /// </summary>
        /// <param name="writer">The reader to read from.</param>
        /// <param name="number">The number to write.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer"/> is null.
        /// </exception>
        public static void WriteBigEndian(this BinaryWriter writer, ushort number)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var bytes = BitConverter.GetBytes(number);
            Array.Reverse(bytes);

            writer.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a 4 byte unsigned integer to the current stream encoded in big 
        /// endian and advances the stream position by 4 bytes.
        /// </summary>
        /// <param name="writer">The reader to read from.</param>
        /// <param name="number">The number to write.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer"/> is null.
        /// </exception>
        public static void WriteBigEndian(this BinaryWriter writer, uint number)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var bytes = BitConverter.GetBytes(number);
            Array.Reverse(bytes);

            writer.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a 8 byte unsigned integer to the current stream encoded in big 
        /// endian and advances the stream position by 8 bytes.
        /// </summary>
        /// <param name="writer">The reader to read from.</param>
        /// <param name="number">The number to write.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer"/> is null.
        /// </exception>
        public static void WriteBigEndian(this BinaryWriter writer, ulong number)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var bytes = BitConverter.GetBytes(number);
            Array.Reverse(bytes);

            writer.Write(bytes, 0, bytes.Length);
        }
    }
}