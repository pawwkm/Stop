using System;
using System.Text;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// A file entry in the FAT.
    /// </summary>
    [Serializer(typeof(FileEntrySerializer))]
    public class FileEntry
    {
        private byte[] shortName = new byte[11];

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEntry"/> class.
        /// </summary>
        public FileEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEntry"/> class.
        /// </summary>
        /// <param name="shortName">The raw short name of 11 bytes.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="shortName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Length of <paramref name="shortName"/> is not 11.
        /// </exception>
        public FileEntry(byte[] shortName)
        {
            if (shortName == null)
                throw new ArgumentNullException(nameof(shortName));
            if (shortName.Length != 11)
                throw new ArgumentOutOfRangeException(nameof(shortName));

            this.shortName = shortName;
        }

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
            get;
            set;
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
            get;
            set;
        }

        /// <summary>
        /// Time the file was created.
        /// </summary>
        public ushort CreationTime
        {
            get;
            set;
        }

        /// <summary>
        /// Time the file was created.
        /// </summary>
        public ushort CreationDate
        {
            get;
            set;
        }

        /// <summary>
        /// The date where this file was last accessed.
        /// This is the date of the last read or write.
        /// </summary>
        public ushort LastAccessDate
        {
            get;
            set;
        }

        /// <summary>
        /// Time of last write. Note that file creation is considered a write.
        /// </summary>
        public ushort WriteTime
        {
            get;
            set;
        }

        /// <summary>
        /// Date of last write. Note that file creation is considered a write.
        /// </summary>
        public ushort WriteDate
        {
            get;
            set;
        }

        /// <summary>
        /// The first cluster of this entry.
        /// </summary>
        public uint FirstCluster
        {
            get
            {
                int original = (FirstClusterHigh << 16) | (FirstClusterLow & 0xffff);

                return (uint)original;
            }
            set
            {
                FirstClusterHigh = (ushort)(value >> 16);
                FirstClusterLow = (ushort)(value & 0xFFFF);
            }
        }

        /// <summary>
        /// The high part of the first cluster.
        /// </summary>
        public ushort FirstClusterHigh
        {
            get;
            set;
        }

        /// <summary>
        /// The low part of the first cluster.
        /// </summary>
        public ushort FirstClusterLow
        {
            get;
            set;
        }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public uint FileSize
        {
            get;
            set;
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

        /// <summary>
        /// Creates a 8.3 name for a string.
        /// </summary>
        /// <param name="name">The string to shorten.</param>
        /// <returns>The 8.3 name.</returns>
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