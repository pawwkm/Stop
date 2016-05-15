using System;
using System.IO;

namespace Stop.FileFormats
{
    /// <summary>
    /// Provides extension methods for the <see cref="BinaryReader"/> class.
    /// </summary>
    internal static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a 2 byte unsigned integer from the current stream using big endian encoding and advances 
        /// the position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>A 2 byte unsigned integer read from this stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public static ushort ReadBigEndianUInt16(this BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(2);
            Array.Reverse(bytes);

            return BitConverter.ToUInt16(bytes, 0);
        }

        /// <summary>
        /// Reads a 4 byte unsigned integer from the current stream using big endian encoding and advances 
        /// the position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>A 4 byte unsigned integer read from this stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public static uint ReadBigEndianUInt32(this BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Reads a 8 byte unsigned integer from the current stream using big endian encoding and advances 
        /// the position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>A 8 byte unsigned integer read from this stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public static ulong ReadBigEndianUInt64(this BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(8);
            Array.Reverse(bytes);

            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}