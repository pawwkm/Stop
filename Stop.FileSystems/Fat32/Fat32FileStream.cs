using System;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// A file in a FAT.
    /// </summary>
    internal class Fat32FileStream : FileStream
    {
        public Fat32FileStream(byte[] buffer, FileEntry entry, long offset) : base(buffer)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            Entry = entry;
            Offset = offset;
        }

        public FileEntry Entry
        {
            get;
            private set;
        }

        public long Offset
        {
            get;
            private set;
        }
    }
}
