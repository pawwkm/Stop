using System;
using Topz.FileSystems.Fat32;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Creates a FAT32 system on the selected partition.
    /// </summary>
    internal class FormatFat32Command : Command
    {
        /// <summary>
        /// Formats the selected partition to FAT32.
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
            else if (context.Partition == null)
                Console.WriteLine("There is no partition selected.");
            else
            {
                BootSector boot = new BootSector();

                boot.SectorsPerCluster = 8;
                if (context.Partition.Sectors <= 16775168) // 8,191MB and below.
                    boot.BytesPerSector = 512;
                else if (context.Partition.Sectors <= 33552384) // 8,192MB to 16,383MB. 
                    boot.BytesPerSector = 1024;
                else if (context.Partition.Sectors <= 67106816) // 16,384MB to 32,767MB.
                    boot.BytesPerSector = 2048;
                else // Larger than 32,768MB
                    boot.BytesPerSector = 4096;

                boot.FatSize = context.Partition.Sectors / boot.BytesPerSector;
                boot.Sectors = context.Partition.Sectors * 512 / boot.BytesPerSector;

                Fat32FileSystem.Create(context.Disk, context.Partition, boot);

                context.Partition.PartitionType = PartitionType.Fat32WithLbaExtensions;
                MasterBootRecordSerializer serializer = new MasterBootRecordSerializer();
                MasterBootRecord mbr = serializer.Deserialize(context.Disk);

                mbr.Partitions[context.Partition.Index - 1] = context.Partition;
                serializer.Serialize(mbr, context.Disk);
            }
        }
    }
}