using System;
using System.IO;

namespace Topz
{
    /// <summary>
    /// Provides extensions to the <see cref="Stream"/> class.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Creates a file and dumps the contents of a stream in it.
        /// </summary>
        /// <param name="source">The source of data to dump.</param>
        /// <param name="path">The path to the file to dump the data in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="path"/> is null.
        /// </exception>
        public static void Dump(this Stream source, string path)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using (var dump = File.Create(path))
            {
                source.Position = 0;
                source.CopyTo(dump);
            }
        }
    }
}