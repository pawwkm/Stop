using System;

namespace Topz.FileSystems
{
    /// <summary>
    /// Describes a partition of a disk.
    /// </summary>
    public class Partition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Partition"/> class.
        /// </summary>
        public Partition()
        {
        }

        /// <summary>
        /// The status of the partition.
        /// </summary>
        public PartitionStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// The type of partition.
        /// </summary>
        public PartitionType PartitionType
        {
            get;
            set;
        }

        /// <summary>
        /// The starting sector of the partition in lba.
        /// </summary>
        public uint Start
        {
            get;
            set;
        }

        /// <summary>
        /// The ending sector of the partition in lba.
        /// </summary>
        public uint End
        {
            get;
            set;
        }

        /// <summary>
        /// Number of sectors between the master boot record and the first sector in the partition.
        /// </summary>
        public uint Offset
        {
            get;
            set;
        }

        /// <summary>
        /// The total amount of sectors in the partition.
        /// </summary>
        public uint Sectors
        {
            get;
            set;
        }
    }
}