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
    }
}