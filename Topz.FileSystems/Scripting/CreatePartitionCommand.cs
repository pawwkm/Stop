﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topz.FileSystems.Scripting
{
    internal class CreatePartitionCommand : Command
    {
        /// <summary>
        /// The partition to work with.
        /// </summary>
        public int Index
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
        /// Creates a partition on the disk for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to work on.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public override void Execute(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Disk == null)
                Console.WriteLine("There is no disk selected.");
            else
            {
                MasterBootRecordSerializer serializer = new MasterBootRecordSerializer();
                MasterBootRecord mbr = serializer.Deserialize(context.Disk);

                mbr.Partitions[Index].Offset = Offset;
                mbr.Partitions[Index].Sectors = Sectors;
            }
        }
    }
}