using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Topz.FileSystems.Fat32
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

        /// <summary>
        /// The short name of the file entry.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is longer than 11 excluding the dot.
        /// </exception>
        /// <remarks>
        /// Long names are not supported!
        /// </remarks>
        public string ShortName
        {
            get
            {
                return Encoding.ASCII.GetString(shortName);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                byte[] bytes = Encoding.ASCII.GetBytes(ToShortName(value));
                Buffer.BlockCopy(bytes, 0, shortName, 0, bytes.Length);
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
                int original = (firstClusterHigh << 16) | (firstClusterLow & 0xffff);

                return (uint)original;
            }
            set
            {
                firstClusterHigh = (ushort)(value >> 16);
                firstClusterLow = (ushort)(value & 0xFFFF);
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

        public static string ToShortName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Length > 11 && name.Length != 12 && name[7] != '.')
                throw new ArgumentOutOfRangeException(nameof(name));

            int dotIndex = name.IndexOf(".");

            if (dotIndex == -1)
                return name.ToUpper().PadRight(11, ' ');

            string shortName = name.Substring(0, dotIndex);
            shortName = shortName.PadRight(8, ' ');

            shortName += name.Substring(dotIndex + 1);
            shortName = shortName.PadRight(11, ' ');

            return shortName.ToUpper();
        }
    }
}