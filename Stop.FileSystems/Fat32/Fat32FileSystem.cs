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
        /// Finds all file entries in the root directory.
        /// </summary>
        /// <returns>All file entries in the root directory.</returns>
        private IEnumerable<FileEntry> GetFileEntriesInRoot()
        {
            uint start = FirstDataSector * boot.BytesPerSector;
            for (uint i = start; i < start + boot.BytesPerCluster; i += 32)
            {
                FileEntry entry = ReadStructure<FileEntry>(i);
                if (!entry.IsThereMoreEntriesInThisFile)
                    break;

                yield return entry;
            }
        }

        private FileEntry FindEntry(string path)
        {
            string[] parts = path.ToLower().Split('\\');            

            return FindEntry(boot.RootCluster, parts, path.EndsWith("\\"));
        }

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

        //private FileEntry FindEntry(string path)
        //{
        //    string[] parts = path.Split('/');
        //    foreach (var entry in GetFileEntriesInRoot())
        //    {
        //        FileEntry e = FindEntry(entry, parts);
        //        if (e != null)
        //            return e;
        //    }

        //    return null;
        //}

        //private FileEntry FindEntry(FileEntry file, string[] parts)
        //{
        //    string part = "";
        //    foreach (var entry in GetEntriesInClusterChain(file.FirstCluster))
        //    {
        //        if (entry.Attributes == FileAttributes.LongName)
        //        {
        //            LongFileEntry first = ReadStructure<LongFileEntry>(Source.Position - 32);
        //            part = first.Name;

        //            for (int i = 0; i < first.NumberOfLongEntriesFollowing - 1; i++)
        //            {
        //                LongFileEntry next = ReadStructure<LongFileEntry>(Source.Position);
        //                part += next.Name;
        //            }
        //        }
        //        else if (parts[0] == part)
        //        {
        //            FileEntry e = ReadStructure<FileEntry>(Source.Position);
        //            if (parts.Length == 1)
        //                return e;

        //            return FindEntry(e, parts.Skip(1).ToArray());
        //        }
        //        else
        //            part = "";
        //    }

        //    return null;
        //}

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
        /// Calculates the first sector the given <paramref name="cluster"/>.
        /// </summary>
        /// <param name="cluster">The cluster to find the first sector of.</param>
        /// <returns>The first sector of the <paramref name="cluster"/>.</returns>
        private uint FirstSectorOfCluster(uint cluster)
        {
            return ((cluster - 2) * boot.SectorsPerCluster) + FirstDataSector;
        }

        private IEnumerable<FileEntry> GetEntriesInClusterChain(uint firstCluster)
        {
            foreach (uint cluster in GetClusterChain(firstCluster))
            {
                uint start = FirstSectorOfCluster(cluster) * boot.BytesPerSector;
                for (uint i = 0; i < boot.BytesPerCluster; i += 32)
                    yield return ReadStructure<FileEntry>(start + i);
            }
        }

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