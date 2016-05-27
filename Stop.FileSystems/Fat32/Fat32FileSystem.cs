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
        private BootSector boot;

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
            boot = ReadStructure<BootSector>(0);
        }

        /// <summary>
        /// The first sector that files and directories are stored.
        /// </summary>
        private uint FirstDataSector
        {
            get
            {
                return boot.ReservedSectors + (boot.Fats * boot.FatSize);
            }
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

            return FindEntry(path) != null;
        }

        /// <summary>
        /// Opens the file specified by the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the file to open.</param>
        /// <returns>The opened file stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// If the given <paramref name="path"/> contains a directory that doesn't exist
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// If the given <paramref name="path"/> is to a file and it doesn't exist
        /// </exception>
        public override Stream Open(string path)
        {
            FileEntry entry = FindEntry(path);
            if (entry == null)
                throw new FileNotFoundException("The file doesn't exist.", nameof(path));

            return new FileStream(GetFileData(entry));
        }

        /// <summary>
        /// Finds the entry with the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the entry.</param>
        /// <returns>The entry if it exists; otherwise null.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        private FileEntry FindEntry(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string[] parts = path.ToLower().Split('\\');

            return FindEntry(boot.RootCluster, parts, path.EndsWith("\\"));
        }

        /// <summary>
        /// Finds the entry of a file or directory.
        /// </summary>
        /// <param name="firstCluster">The first cluster to search.</param>
        /// <param name="parts">The parts of the path.</param>
        /// <param name="isDirectory">True if the expected entry is a directory.</param>
        /// <returns>The entry if it exists; otherwise null.</returns>
        private FileEntry FindEntry(uint firstCluster, string[] parts, bool isDirectory)
        {
            foreach (FileEntry entry in GetEntriesInClusterChain(firstCluster))
            {
                if (!entry.Attributes.HasFlag(FileAttributes.LongName))
                    continue;

                Source.Position -= 32;
                string name = ReadLongName().ToLower();

                FileEntry e = ReadStructure<FileEntry>(Source.Position);
                if (e.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (name != parts[0])
                        continue;

                    if (parts.Length > 1)
                    {
                        FileEntry result = FindEntry(e.FirstCluster, parts.Skip(1).ToArray(), isDirectory);
                        if (result != null)
                            return result;
                    }
                    else if (!isDirectory)
                        return e;
                }
                else
                {
                    if (isDirectory)
                        continue;

                    if (parts.Length == 1 && name == parts[0])
                        return e;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the long name at the current position.
        /// </summary>
        /// <returns>The long name at the current position.</returns>
        private string ReadLongName()
        {
            LongFileEntry first = ReadStructure<LongFileEntry>(Source.Position);

            string name = first.Name;
            for (int i = 0; i < first.NumberOfLongEntriesFollowing - 1; i++)
            {
                LongFileEntry next = ReadStructure<LongFileEntry>(Source.Position);
                name += next.Name;
            }

            return name;
        }

        /// <summary>
        /// Calculates the first sector the given <paramref name="cluster"/>.
        /// </summary>
        /// <param name="cluster">The cluster to find the first sector of.</param>
        /// <returns>The first sector of the <paramref name="cluster"/>.</returns>
        private uint FirstSectorOfCluster(uint cluster)
        {
            return ((cluster - 2) * boot.SectorsPerCluster) + FirstDataSector;
        }

        /// <summary>
        /// Gets all entries in a cluster chain.
        /// </summary>
        /// <param name="firstCluster">The first cluster in the chain.</param>
        /// <returns>All entries in a cluster chain.</returns>
        private IEnumerable<FileEntry> GetEntriesInClusterChain(uint firstCluster)
        {
            foreach (uint cluster in GetClusterChain(firstCluster))
            {
                uint start = FirstSectorOfCluster(cluster) * boot.BytesPerSector;
                for (uint i = 0; i < boot.BytesPerCluster; i += 32)
                    yield return ReadStructure<FileEntry>(start + i);
            }
        }

        /// <summary>
        /// Gets the data of a file.
        /// </summary>
        /// <param name="entry">The file entry of the file.</param>
        /// <returns>The bytes of the file.</returns>
        private byte[] GetFileData(FileEntry entry)
        {
            byte[] bytes = new byte[entry.FileSize];
            uint amount = entry.FileSize;
            int count = 0;

            foreach (uint cluster in GetClusterChain(entry.FirstCluster))
            {
                uint start = FirstSectorOfCluster(cluster) * boot.BytesPerSector;
                byte[] chunk = ReadData(start, boot.BytesPerCluster);

                uint size = amount > boot.BytesPerCluster ? boot.BytesPerCluster : amount;
                Buffer.BlockCopy(chunk, 0, bytes, count * (int)boot.BytesPerCluster, (int)size);

                if (amount - boot.BytesPerCluster <= 0)
                    break;

                amount -= boot.BytesPerCluster;
            }

            return bytes;
        }

        /// <summary>
        /// Gets a cluster chain.
        /// </summary>
        /// <param name="firstCluster">The first cluster in chain.</param>
        /// <returns>
        /// <paramref name="firstCluster"/> then the clusters following that one, if any.
        /// </returns>
        private IEnumerable<uint> GetClusterChain(uint firstCluster)
        {
            yield return firstCluster;

            uint cluster = firstCluster;
            while (cluster != 0)
            {
                Source.Position = boot.ReservedSectors + cluster * 4 / boot.BytesPerSector;
                byte[] bytes = new byte[4];

                Source.Read(bytes, 0, bytes.Length);

                cluster = BitConverter.ToUInt16(bytes, 0);
                if (cluster == 0)
                    break;

                yield return cluster;
            }
        }
    }
}