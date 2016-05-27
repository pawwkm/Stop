using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// A file entry in the FAT with a long name.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    internal class LongFileEntry
    {
        private byte order;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        private byte[] name1 = new byte[10];

        private byte attribute;

        private byte type;

        private byte checksum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        private byte[] name2 = new byte[12];

        private ushort zero;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] name3 = new byte[4];

        /// <summary>
        /// If true there are no more long file entries after this one.
        /// </summary>
        public bool IsLastEntry
        {
            get
            {
                return NumberOfLongEntriesFollowing == 0;
            }
        }

        public int NumberOfLongEntriesFollowing
        {
            get
            {
                return order - 0x40;
            }
        }

        /// <summary>
        /// The name of this long entry.
        /// </summary>
        public string Name
        {
            get
            {
                var bytes = name1.Concat(name2).Concat(name3).ToArray();

                return Encoding.Unicode.GetString(bytes).TrimEnd('\0', '\uFFFF');
            }
        }
    }
}