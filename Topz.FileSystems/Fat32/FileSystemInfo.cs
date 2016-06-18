using System.Runtime.InteropServices;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// The structure pointed to by <see cref="BootSector.FileSystemInfoSector"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
    public class FileSystemInfo
    {
        private uint leadSignature = 0x41615252;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 480)]
        private byte[] reserved1 = new byte[480];

        private uint structSignature = 0x61417272;

        private uint lastFreeCluster = 0xFFFFFFFF;

        private uint nextFreeCluster = 0xFFFFFFFF;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        private byte[] reserved2 = new byte[12];

        private uint trailSignature = 0xAA550000;

        /// <summary>
        /// True if this is indeed a <see cref="FileSystemInfo"/>.
        /// </summary>
        public bool IsFileSystemInfo
        {
            get
            {
                return leadSignature == 0x41615252 &&
                       structSignature == 0x61417272 &&
                       trailSignature == 0xAA550000;
            }
        }

        /// <summary>
        /// Contains the last known free cluster count on
        /// the volume.If the value is 0xFFFFFFFF,
        /// then the free count is unknown and must be
        /// computed.Any other value can be used, but
        /// is not necessarily correct.It should be range
        /// checked at least to make sure it is &lt;= volume
        /// cluster count.
        /// </summary>
        public uint LastFreeCluster
        {
            get
            {
                return lastFreeCluster;
            }
            set
            {
                lastFreeCluster = value;
            }
        }

        /// <summary>
        /// This is a hint for the FAT driver.It indicates the cluster number at
        /// which the driver should start looking for free clusters.Because a
        /// FAT32 FAT is large, it can be rather time consuming if there are a
        /// lot of allocated clusters at the start of the FAT and the driver starts
        /// looking for a free cluster starting at cluster 2. Typically this value is
        /// set to the last cluster number that the driver allocated. If the value is
        /// 0xFFFFFFFF, then there is no hint and the driver should start
        /// looking at cluster 2. Any other value can be used, but should be
        /// checked first to make sure it is a valid cluster number for the
        /// volume.
        /// </summary>
        public uint NextFreeCluster
        {
            get
            {
                return nextFreeCluster;
            }
            set
            {
                nextFreeCluster = value;
            }
        }
    }
}