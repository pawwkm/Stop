using System;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// A file in a FAT32.
    /// </summary>
    internal class Fat32FileStream : FileStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fat32FileStream"/> class.
        /// </summary>
        /// <param name="buffer">The data of the file.</param>
        /// <param name="entry">The file entry in the fat.</param>
        /// <param name="offset">The absolute address of the file entry.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> or <paramref name="entry"/> is null.
        /// </exception>
        public Fat32FileStream(byte[] buffer, FileEntry entry, long offset) : base(buffer)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            Entry = entry;
            Offset = offset;
        }

        /// <summary>
        /// The file entry in the fat.
        /// </summary>
        public FileEntry Entry
        {
            get;
            private set;
        }

        /// <summary>
        /// The absolute address of the file entry.
        /// </summary>
        public long Offset
        {
            get;
            private set;
        }
    }
}