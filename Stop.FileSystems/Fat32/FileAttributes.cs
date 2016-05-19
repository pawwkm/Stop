using System;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// Attributes used by <see cref="FileEntry.Attributes"/>.
    /// </summary>
    [Flags]
    internal enum FileAttributes : byte
    {
        /// <summary>
        /// The file is read only.
        /// </summary>
        ReadOnly = 0x01,

        /// <summary>
        /// The file is hidden.
        /// </summary>
        Hidden = 0x02,

        /// <summary>
        /// This file is part of the system.
        /// </summary>
        System = 0x04, 

        /// <summary>
        /// This file is the volume id.
        /// </summary>
        VolumeId = 0x08,

        /// <summary>
        /// The file is a directory.
        /// </summary>
        Directory = 0x10,

        /// <summary>
        /// The file is an archive.
        /// </summary>
        Archive = 0x20,

        /// <summary>
        /// The file is a long name.
        /// </summary>
        LongName = ReadOnly | Hidden | System | VolumeId
    }
}