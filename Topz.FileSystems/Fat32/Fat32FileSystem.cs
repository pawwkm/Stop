using Pote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// Represents a Fat32.
    /// </summary>
    public class Fat32FileSystem : FileSystem
    {
        private class PathSegment
        {
            public string Name
            {
                get;
                set;
            }

            public bool IsDirectory
            {
                get;
                set;
            }
        }

        private BootSector boot;

        private FileSystemInfo info;

        private List<Fat32FileStream> openedFiles = new List<Fat32FileStream>();
        
        private uint this[uint index]
        {
            get
            {
                if (index > boot.Clusters - 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                
                Source.Position = boot.ReservedSectors * boot.BytesPerSector + index * 4;

                byte[] bytes = new byte[4];
                Source.Read(bytes, 0, bytes.Length);

                return BitConverter.ToUInt32(bytes, 0) & 0x0FFFFFFF;
            }
            set
            {
                if (index > boot.Clusters - 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                
                Source.Position = boot.ReservedSectors * boot.BytesPerSector + index * 4;

                byte[] bytes = BitConverter.GetBytes(value);
                Source.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Creates Fat32 file system on the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream the file system is written on.</param>
        /// <param name="boot">The boot sector of the file system.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> or <paramref name="boot"/> is null.
        /// </exception>
        public static void Create(Stream stream, BootSector boot)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (boot == null)
                throw new ArgumentNullException(nameof(boot));

            if (boot.Fats != 1)
                throw new ArgumentOutOfRangeException(nameof(boot), "This must be 1.");

            byte[] bytes = Structures.ToBytes(boot);
            stream.Position = 0;
            stream.Write(bytes, 0, bytes.Length);

            bytes = Structures.ToBytes(new FileSystemInfo());
            stream.Position = boot.FileSystemInfoSector * boot.BytesPerSector;
            stream.Write(bytes, 0, bytes.Length);

            FileEntry volume = new FileEntry();
            volume.ShortName = "SD Card";
            volume.Attributes = FileAttributes.VolumeId;

            uint firstDataSector = boot.ReservedSectors + (boot.Fats * boot.FatSize);
            uint firstByteOfRootCluster = ((boot.RootCluster - 2) * boot.SectorsPerCluster) + firstDataSector;

            bytes = Structures.ToBytes(volume);
            stream.Position = firstByteOfRootCluster;
            stream.Write(bytes, 0, bytes.Length);

            // Allocate the root cluster.
            bytes = BitConverter.GetBytes(0xFFFFFFFF);
            stream.Position = boot.ReservedSectors * boot.BytesPerSector + boot.RootCluster * 4;
            stream.Write(bytes, 0, bytes.Length);

            // Place signature.
            bytes = BitConverter.GetBytes((ushort)0xAA55);
            stream.Position = 510;
            stream.Write(bytes, 0, bytes.Length);

            // Reach the end of the disk to allocated it.
            stream.Position = boot.Sectors * boot.BytesPerSector;
            stream.WriteByte(0);
        }

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
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Exist(path))
                throw new FileNotFoundException("The file doesn't exist.", nameof(path));

            FileEntry entry = FindEntry(path);
            long position = Source.Position - 32;
            byte[] data = GetFileData(entry);

            Fat32FileStream stream = new Fat32FileStream(data, entry, position);
            stream.AfterFlush += Flush;
            stream.AfterClose += Close;

            openedFiles.Add(stream);

            return stream;
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

            List<PathSegment> segments = new List<PathSegment>();
            string[] parts = path.Split('\\');
            
            for (int i = 0; i < parts.Length; i++)
            {
                PathSegment segment = new PathSegment();
                segment.Name = FileEntry.ToShortName(parts[i]);

                if (i == parts.Length - 1)
                {
                    if (path.EndsWith("\\"))
                        segment.IsDirectory = true;
                }
                else
                    segment.IsDirectory = true;

                segments.Add(segment);
            }

            return FindEntry(boot.RootCluster, segments);
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
                    if (entry.ShortName != FileEntry.ToShortName(parts[0]) || !isDirectory)
                        continue;

                    if (parts.Length == 1)
                        return entry;

                    return FindEntry(entry.FirstCluster, parts.Skip(1).ToArray(), isDirectory);
                }
                else
                {
                    if (isDirectory)
                        continue;

                    if (parts.Length == 1 && entry.ShortName == FileEntry.ToShortName(parts[0]))
                        return entry;
                }
            }

            return null;
        }

        private FileEntry FindEntry(uint firstCluster, List<PathSegment> segments)
        {
            foreach (FileEntry entry in GetEntriesInClusterChain(firstCluster))
            {
                if (entry.Attributes.HasFlag(FileAttributes.LongName))
                    continue;
                if (entry.Attributes.HasFlag(FileAttributes.VolumeId))
                    continue;

                if (entry.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (entry.ShortName != segments[0].Name || !segments[0].IsDirectory)
                        continue;

                    if (segments.Count == 1)
                        return entry;

                    return FindEntry(entry.FirstCluster, segments.Skip(1).ToList());
                }
                else
                {
                    if (segments[0].IsDirectory)
                        continue;

                    if (segments.Count == 1 && entry.ShortName == segments[0].Name)
                        return entry;
                }
            }

            return null;
        }

        /// <summary>
        /// Calculates the first sector of the given <paramref name="cluster"/>.
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
                uint size = amount > boot.BytesPerCluster ? boot.BytesPerCluster : amount;

                Source.Position = start;

                Source.Read(bytes, count * (int)boot.BytesPerCluster, (int)size);
                amount -= boot.BytesPerCluster;
                count++;
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
            uint cluster = firstCluster;
            while (cluster < 0x0FFFFFF8 && cluster != 0)
            {
                yield return cluster;

                cluster = this[cluster];
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
        /// <param name="previousCluster">
        /// The cluster that points to the new free cluster.
        /// The cluster chain is not updated if this is negative.
        /// </param>
        /// <returns>A free cluster.</returns>
        private uint GetFreeCluster(long previousCluster = -1)
        {
            if (info.NextFreeCluster == 0xFFFFFFFF)
                Lol();

            uint freeCluster = info.NextFreeCluster;
            this[freeCluster] = 0x0FFFFFFF; // End of chain.

            Lol();
            if (previousCluster >= 0)
                this[(uint)previousCluster] = freeCluster;

            return freeCluster;
        }

        private void Lol()
        {
            for (uint i = 2; i < uint.MaxValue; i++) // Start looking at cluster 2. Cluster 0 and 1 are special.
            {
                if (this[i] == 0)
                {
                    info.LastFreeCluster = info.NextFreeCluster;
                    info.NextFreeCluster = i;

                    return;
                }
            }
        }

        /// <summary>
        /// Flushes a file's contents.
        /// </summary>
        /// <param name="sender">The file to flush.</param>
        /// <param name="e">This is not used.</param>
        /// <exception cref="InvalidOperationException">
        /// The file has been closed.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Overriding contents not supported.
        /// </exception>
        private void Flush(object sender, EventArgs e)
        {
            Fat32FileStream file = sender as Fat32FileStream;
            if (!openedFiles.Contains(file))
                throw new InvalidOperationException("The file has been closed.");

            if (file.Entry.FileSize != 0)
                throw new NotSupportedException("Overriding contents not supported.");

            file.Entry.FileSize = (uint)file.Length;
            file.Position = 0;

            byte[] buffer = new byte[boot.BytesPerCluster];
            long cluster = -1;

            while (file.Position != file.Length)
            {
                cluster = GetFreeCluster(cluster);
                if (file.Position == 0)
                    file.Entry.FirstCluster = (uint)cluster;

                Source.Position = FirstSectorOfCluster((uint)cluster) * boot.BytesPerSector;

                int bytesRead = file.Read(buffer, 0, buffer.Length);
                Source.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Closes a file.
        /// </summary>
        /// <param name="sender">The file to flush.</param>
        /// <param name="e">This is not used.</param>
        /// <exception cref="InvalidOperationException">
        /// The file has already been closed.
        /// </exception>
        private void Close(object sender, EventArgs e)
        {
            Fat32FileStream stream = sender as Fat32FileStream;
            if (!openedFiles.Contains(stream))
                throw new InvalidOperationException("The file has already been closed.");

            WriteStructure(stream.Offset, stream.Entry);

            openedFiles.Remove(stream);
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