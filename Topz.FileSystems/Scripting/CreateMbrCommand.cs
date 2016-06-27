using System;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Creates a master boot record on a disk. 
    /// </summary>
    internal class CreateMbrCommand : Command
    {
        /// <summary>
        /// Creates an mbr on the disk for the given <paramref name="context"/>.
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
                Partition[] partitions =
                {
                    new Partition(1),
                    new Partition(2),
                    new Partition(3),
                    new Partition(4)
                };

                MasterBootRecord mbr = new MasterBootRecord(partitions);
                MasterBootRecordSerializer serializer = new MasterBootRecordSerializer();

                serializer.Serialize(mbr, context.Disk);
            }
        }
    }
}