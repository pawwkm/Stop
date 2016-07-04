using System.Diagnostics.CodeAnalysis;

namespace Topz.FileSystems
{
    /// <summary>
    /// The type of partition.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = Justifications.UnderlyingTypeIsRequired)]
    public enum PartitionType : byte
    {
        /// <summary>
        /// There is no file system in the partition.
        /// </summary>
        Empty = 0x00,

        /// <summary>
        /// 32-bit FAT (Partition Up to 2048GB).
        /// </summary>
        Fat32 = 0x0B,

        /// <summary>
        /// Same as <see cref="Fat32"/>, but uses LBA 0x13 extensions.
        /// </summary>
        Fat32WithLbaExtensions = 0x0C
    }
}