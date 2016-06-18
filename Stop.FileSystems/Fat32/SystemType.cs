using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// A type of FAT.
    /// </summary>
    public enum SystemType
    {
        /// <summary>
        /// The system is FAT12.
        /// </summary>
        Fat12,

        /// <summary>
        /// The system is FAT16.
        /// </summary>
        Fat16,

        /// <summary>
        /// The system is FAT32.
        /// </summary>
        Fat32
    }
}