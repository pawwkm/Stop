using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pote;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// Represents a Fat32.
    /// </summary>
    public class Fat32FileSystem : FileSystem
    {
        private BootSector boot;

        private FileSystemInfo info;

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
            info = ReadStructure<FileSystemInfo>(boot.FileSystemInfoSector * boot.BytesPerSector);

            if (!info.IsFileSystemInfo)
                throw new ArgumentException("File System Info is corrupted.", nameof(stream));
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
        /// Creates a file or a directory in the file system.
        /// </summary>
        /// <param name="path">The path to the new file or directory.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        public override void Create(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (Exist(path))
                throw new ArgumentException("A file or directory with the same name exists.", nameof(path));

            bool isDirectory = path.EndsWith("\\");
            string[] parts = path.Split('\\');

            if (parts.Length > 1)
            {
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string p = string.Join("\\", parts, 0, i) + '\\';
                    if (!Exist(p))
                        Create(p);
                }
            }

            uint cluster = 0;
            uint firstClusterOfParent = 0;

            if (parts.Length > 1)
            {
                string parent = string.Join("\\", parts, 0, parts.Length - 1) + '\\';

                firstClusterOfParent = FindEntry(parent).FirstCluster;
                cluster = GetClusterChain(firstClusterOfParent).Last();
            }
            else
                cluster = GetClusterChain(boot.RootCluster).Last();

            if (IsClusterFull(cluster))
                cluster = GetFreeCluster();

            FileEntry entry = new FileEntry();
            if (isDirectory)
            {
                entry.Attributes = FileAttributes.Directory;
                entry.FirstCluster = GetFreeCluster();

                FileEntry dot = new FileEntry();
                dot.FirstCluster = entry.FirstCluster;
                dot.Attributes = FileAttributes.Directory;

                FileEntry dotdot = new FileEntry();
                dotdot.Attributes = FileAttributes.Directory;
                dotdot.FirstCluster = firstClusterOfParent;

                Write(dot, entry.FirstCluster, 0);
                Write(dotdot, entry.FirstCluster, 32);
            }

            entry.ShortName = parts.Last();

            uint offset = (uint)GetSpaceOffsetForEntryInCluster(cluster);
            Write(entry, cluster, offset);
        }

        /// <summary>
        /// Writes an entry to the disk.
        /// </summary>
        /// <param name="entry">The entry to write down.</param>
        /// <param name="cluster">The cluster the entry is written into.</param>
        /// <param name="offset">The offset from the beginning cluster, in bytes.</param>
        private void Write(FileEntry entry, uint cluster, uint offset)
        {
            uint position = FirstSectorOfCluster(cluster) * boot.BytesPerSector + offset;
            int value = Source.ReadByte();

            WriteStructure(position, entry);
            if (value == 0)
                Source.WriteByte(0);
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

            bool isDirectory = path.EndsWith("\\");
            string[] parts = path.ToUpper().Split('\\');

            if (isDirectory)
                parts = parts.Take(parts.Length - 1).ToArray();

            return FindEntry(boot.RootCluster, parts, isDirectory);
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
                if (entry.Attributes.HasFlag(FileAttributes.LongName))
                    continue;
                if (entry.Attributes.HasFlag(FileAttributes.VolumeId))
                    continue;

                if (entry.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (entry.ShortName != parts[0] || !isDirectory)
                        continue;

                    if (parts.Length == 1)
                        return entry;

                    return FindEntry(entry.FirstCluster, parts.Skip(1).ToArray(), isDirectory);
                }
                else
                {
                    if (isDirectory)
                        continue;

                    if (parts.Length == 1 && entry.ShortName == parts[0])
                        return entry;
                }
            }

            return null;
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

                cluster = BitConverter.ToUInt32(bytes, 0);
                if (cluster == 0)
                    break;

                yield return cluster;
            }
        }

        /// <summary>
        /// Checks if a cluster is full.
        /// </summary>
        /// <param name="cluster">The cluster to check.</param>
        /// <returns>True if the <paramref name="cluster"/>; otherwise false.</returns>
        private bool IsClusterFull(uint cluster)
        {
            return GetSpaceOffsetForEntryInCluster(cluster) == -1;
        }

        /// <summary>
        /// Gets the offset where there is a 32 byte chunk of free space
        /// in the given <paramref name="cluster"/>.
        /// </summary>
        /// <param name="cluster">The cluster to search in.</param>
        /// <returns>
        /// The offset where there is a 32 byte chunk of free space
        /// in the given <paramref name="cluster"/> if there is one;
        /// otherwise -1.
        /// </returns>
        private int GetSpaceOffsetForEntryInCluster(uint cluster)
        {
            uint start = FirstSectorOfCluster(cluster) * boot.BytesPerSector;
            uint end = start + boot.BytesPerCluster;
            
            for (uint i = start; i < end; i += 32)
            {
                Source.Position = i;
                byte value = (byte)Source.ReadByte();

                if (value.IsOneOf(0, 0xE5))
                    return (int)(i - start);
            }

            return -1;
        }

        /// <summary>
        /// Gets a free cluster.
        /// </summary>
        /// <returns>A free cluster.</returns>
        private uint GetFreeCluster()
        {
            uint freeCluster = info.NextFreeCluster;
            info.LastFreeCluster = info.NextFreeCluster;

            // Start looking at cluster 2.
            Source.Position = boot.ReservedSectors + 2 * 4 / boot.BytesPerSector;
            byte[] bytes = new byte[4];

            do
            {
                Source.Read(bytes, 0, bytes.Length);
                info.NextFreeCluster = BitConverter.ToUInt32(bytes, 0);

                if (info.NextFreeCluster >= 0x0FFFFFF8)
                    break;
            }
            while (info.NextFreeCluster != 0x0FFFFFF8);

            return freeCluster;
        }

        /// <summary>
        /// Writes crucial structures to disk.
        /// </summary>
        ~Fat32FileSystem()
        {
            WriteStructure(0, boot);
            WriteStructure(boot.FileSystemInfoSector * boot.BytesPerSector, info);
        }
    }
}