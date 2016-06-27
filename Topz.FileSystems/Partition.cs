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
        /// <param name="index">The index of the partition in the mbr, starting at 1.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not from 1 to 4.
        /// </exception>
        public Partition(int index)
        {
            if (1 > index || 4 < index)
                throw new ArgumentOutOfRangeException(nameof(index), "Must be from 1 to 4.");

            Index = index;
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

        /// <summary>
        /// The index of the partition in the mbr.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }
    }
}