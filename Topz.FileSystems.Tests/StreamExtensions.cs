using System.IO;

namespace Topz.FileSystems
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
        public static void Dump(this Stream source, string path)
        {
            using (Stream dump = File.Create(path))
            {
                source.Position = 0;
                source.CopyTo(dump);
            }
        }
    }
}
