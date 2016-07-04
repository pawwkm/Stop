using System;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Selects and reads a partition from the mbr.
    /// </summary>
    internal class SelectPartitionCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectPartitionCommand"/> class.
        /// </summary>
        /// <param name="index">The index of the partition to select.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not from 1 to 4.
        /// </exception>
        public SelectPartitionCommand(int index)
        {
            if (index < 1 || index > 4)
                throw new ArgumentOutOfRangeException(nameof(index), "Must be from 1 to 4.");

            Index = index;
        }

        /// <summary>
        /// The index of the partition to select.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        /// Selects a partition for the given <paramref name="context"/>.
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

                context.Partition = mbr.Partitions[Index - 1];
            }
        }
    }
}