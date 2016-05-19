using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// Represents a Fat32.
    /// </summary>
    public class Fat32FileSystem : FileSystem
    {
        private BootSector bootSector;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fat32FileSystem"/> class.
        /// </summary>
        /// <param name="stream">The stream containing the image of the file system.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="stream"/> cannot be read or written or seeked.
        /// </exception>
        public Fat32FileSystem(Stream stream) : base(stream)
        {
            bootSector = ReadStructure<BootSector>(0);
        }

        /// <summary>
        /// Checks if a directory or file exists in the file system.
        /// </summary>
        /// <param name="path">The path of the directory or file.</param>
        /// <returns>true if the directory or file exists; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        public override bool Exist(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return false;
        }

        /// <summary>
        /// Finds all file entries in the root directory.
        /// </summary>
        /// <returns>All file entries in the root directory.</returns>
        private IEnumerable<FileEntry> GetFileEntriesInRoot()
        {
            uint bytesInCluster = (uint)(bootSector.SectorsPerCluster * bootSector.BytesPerSector);
            uint start = FirstDataSector * bootSector.BytesPerSector;

            for (uint i = start; i < start + bytesInCluster; i += 32)
            {
                FileEntry entry = ReadStructure<FileEntry>(i);
                if (!entry.IsThereMoreEntriesInThisFile)
                    break;

                yield return entry;
            }
        }

        /// <summary>
        /// The first sector that files and directories are stored.
        /// </summary>
        private uint FirstDataSector
        {
            get
            {
                return bootSector.ReservedSectors + (bootSector.Fats * bootSector.FatSize); 
            }
        }

        /// <summary>
        /// Calculates the first sector the given <paramref name="cluster"/>.
        /// </summary>
        /// <param name="cluster">The cluster to find the first sector of.</param>
        /// <returns>The first sector of the <paramref name="cluster"/>.</returns>
        private uint FirstSectorOfCluster(uint cluster)
        {
            return ((cluster - 2) * bootSector.SectorsPerCluster) + FirstDataSector;
        }
    }
}