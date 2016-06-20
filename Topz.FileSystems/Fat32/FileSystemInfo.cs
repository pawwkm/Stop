namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// The structure pointed to by <see cref="BootSector.FileSystemInfoSector"/>.
    /// </summary>
    [Serializer(typeof(FileSystemInfoSerializer))]
    public class FileSystemInfo
    {
        /// <summary>
        /// True if this is indeed a <see cref="FileSystemInfo"/>.
        /// </summary>
        public bool IsFileSystemInfo
        {
            get
            {
                return LeadSignature == 0x41615252 &&
                       StructSignature == 0x61417272 &&
                       TrailSignature == 0xAA550000;
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
            get;
            set;
        } = 0xFFFFFFFF;

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
            get;
            set;
        } = 0xFFFFFFFF;

        /// <summary>
        /// The first signature.
        /// </summary>
        public uint LeadSignature
        {
            get;
            set;
        } = 0x41615252;

        /// <summary>
        /// The signature in the middle.
        /// </summary>
        public uint StructSignature
        {
            get;
            set;
        } = 0x61417272;

        /// <summary>
        /// The trailing signature.
        /// </summary>
        public uint TrailSignature
        {
            get;
            set;
        } = 0xAA550000;
    }
}