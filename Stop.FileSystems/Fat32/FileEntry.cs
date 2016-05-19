using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// A file entry in the FAT.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    internal class FileEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        private byte[] shortName = new byte[11];

        private FileAttributes attributes;

        private byte reserved;

        private byte creationTimeMilliseconds;

        private ushort creationTime;

        private ushort creationDate;

        private ushort lastAccessDate;

        private ushort firstClusterHigh;

        private ushort writeTime;

        private ushort writeDate;

        private ushort firstClusterLow;

        private uint fileSize;

        public string ShortName
        {
            get
            {
                return Encoding.ASCII.GetString(shortName);
            }
        }

        /// <summary>
        /// The attributes of the entry.
        /// </summary>
        public FileAttributes Attributes
        {
            get
            {
                return attributes;
            }
            set
            {
                attributes = value;
            }
        }

        /// <summary>
        /// Millisecond stamp at file creation time. This field actually
        /// contains a count of tenths of a second. The granularity of the
        /// seconds part of <see cref="CreationTime"/> is 2 seconds so this 
        /// field is a count of tenths of a second and its valid value range 
        /// is 0-199 inclusive.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is over 199.
        /// </exception>
        public byte CreationTimeMilliseconds
        {
            get
            {
                return creationTimeMilliseconds;
            }
            set
            {
                if (value > 199)
                    throw new ArgumentOutOfRangeException(nameof(value), "The valid range is 0-199 inclusive.");

                creationTimeMilliseconds = value;
            }
        }

        /// <summary>
        /// Time the file was created.
        /// </summary>
        public ushort CreationTime
        {
            get
            {
                return creationTime;
            }
            set
            {
                creationTime = value;
            }
        }

        /// <summary>
        /// Time the file was created.
        /// </summary>
        public ushort CreationDate
        {
            get
            {
                return creationDate;
            }
            set
            {
                creationDate = value;
            }
        }

        /// <summary>
        /// The date where this file was last accessed.
        /// This is the date of the last read or write.
        /// </summary>
        public ushort LastAccessDate
        {
            get
            {
                return lastAccessDate;
            }
            set
            {
                lastAccessDate = value;
            }
        }

        /// <summary>
        /// Time of last write. Note that file creation is considered a write.
        /// </summary>
        public ushort WriteTime
        {
            get
            {
                return writeTime;
            }
            set
            {
                writeTime = value;
            }
        }

        /// <summary>
        /// Date of last write. Note that file creation is considered a write.
        /// </summary>
        public ushort WriteDate
        {
            get
            {
                return writeDate;
            }
            set
            {
                writeDate = value;
            }
        }

        /// <summary>
        /// The first cluster of this entry.
        /// </summary>
        public uint FirstCluster
        {
            get
            {
                return (uint)(firstClusterHigh << 16 | (firstClusterLow & 0x0000FFFF));
            }
            set
            {
                firstClusterHigh = (ushort)(value & 0xFFFF0000);
                firstClusterLow = (ushort)(value & 0x0000FFFF);
            }
        }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public uint FileSize
        {
            get
            {
                return fileSize;
            }
            set
            {
                fileSize = value;
            }
        }

        /// <summary>
        /// If true there are more
        /// </summary>
        public bool IsThereMoreEntriesInThisFile
        {
            get
            {
                return shortName[0] != 0;
            }
        }
    }
}