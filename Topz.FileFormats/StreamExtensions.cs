using System;
using System.IO;

namespace Topz.FileFormats
{
    /// <summary>
    /// Provides extension methods to the <see cref="Stream"/> class.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// The number of bytes left in given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>The number of bytes left in the stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public static long BytesLeft(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return stream.Length - stream.Position;
        }

        /// <summary>
        /// Checks if there is no more bytes left in the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if there is no more bytes left in the <paramref name="stream"/>; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public static bool IsEndOfStream(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return stream.Position == stream.Length;
        }
    }
}