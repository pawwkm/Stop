using System.IO;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// The context of a <see cref="Script"/>
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The selected disk. If no disk has been selected this is null.
        /// </summary>
        public Stream Disk
        {
            get;
            set;
        }

        /// <summary>
        /// The currently selected partition of the <see cref="Disk"/>; otherwise null.
        /// </summary>
        public Partition Partition
        {
            get;
            set;
        }
    }
}