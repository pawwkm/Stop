using System;
using System.Collections.Generic;

namespace Topz.FileSystems
{
    /// <summary>
    /// A master boot record of a disk.
    /// </summary>
    [Serializer(typeof(MasterBootRecordSerializer))]
    public class MasterBootRecord
    {
        private Partition[] partitions = new Partition[4];

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterBootRecord"/> class.
        /// </summary>
        /// <param name="partitions">The 4 partitions of the mbr.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="partitions"/> is null or one of the 4 partitions is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="partitions"/> has more or less then 4 partitions.
        /// </exception>
        public MasterBootRecord(IList<Partition> partitions)
        {
            if (partitions == null)
                throw new ArgumentNullException(nameof(partitions));
            if (partitions.Count != 4)
                throw new ArgumentOutOfRangeException("There must be exactly 4 partitions.", nameof(partitions));

            for (int i = 0; i < 4; i++)
            {
                if (partitions[i] == null)
                    throw new ArgumentNullException(nameof(partitions));

                this.partitions[i] = partitions[i];
            }
        }

        /// <summary>
        /// The 4 partitions of the Mbr.
        /// </summary>
        public Partition[] Partitions
        {
            get
            {
                return partitions;
            }
        }
    }
}