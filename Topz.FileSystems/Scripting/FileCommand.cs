using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topz.FileSystems.Fat32;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Base class for commands that opererates on <see cref="FileSystem"/>.
    /// </summary>
    internal abstract class FileCommand : Command
    {
        /// <summary>
        /// The file system to work on.
        /// If null, the system is not known.
        /// </summary>
        protected FileSystem FileSystem
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads the file system from a context.
        /// </summary>
        /// <param name="context">The context to work with.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public override void Execute(Context context)
        {
            if (Fat32FileSystem.IsFat32FileSystem(context.Disk, context.Partition))
                FileSystem = new Fat32FileSystem(context.Disk, context.Partition);
            else
                Console.WriteLine("File system not supported.");
        }
    }
}