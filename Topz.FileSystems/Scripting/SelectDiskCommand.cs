using System;
using System.IO;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Selects a disk
    /// </summary>
    internal class SelectDiskCommand : Command
    {
        private bool isPhysical;

        private int id;

        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDiskCommand"/> class.
        /// </summary>
        /// <param name="path">The path where the image should be stored.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> 
        /// </exception>
        public SelectDiskCommand(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDiskCommand"/> class.
        /// </summary>
        /// <param name="physicalDisk">The number of the physical disk to access.</param>
        public SelectDiskCommand(int physicalDisk)
        {
            id = physicalDisk;
            isPhysical = true;
        }

        /// <summary>
        /// Selects a physical disk or a file.
        /// </summary>
        /// <param name="context">The context of the script.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public override void Execute(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Disk != null)
                context.Disk.Dispose();

            if (isPhysical)
                context.Disk = new DiskStream(id, true);
            else
                context.Disk = File.Create(path);
        }
    }
}