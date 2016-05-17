using System;

namespace Stop.FileSystems
{
    /// <summary>
    /// The requested access to the file or device, which can be summarized as read, write, both or neither zero).
    /// </summary>
    [Flags]
    public enum EFileAccess : uint
    {
        /// <summary>
        /// The right to read file attributes.
        /// </summary>
        FILE_READ_ATTRIBUTES = 0x0080,

        /// <summary>
        /// The right to write file attributes.
        /// </summary>
        FILE_WRITE_ATTRIBUTES = 0x0100,

        /// <summary>
        /// Read access.
        /// </summary>
        GenericRead = 0x80000000,
        
        /// <summary>
        /// Write access.
        /// </summary>
        GenericWrite = 0x40000000,

        /// <summary>
        /// Execute access.
        /// </summary>
        GenericExecute = 0x20000000,

        /// <summary>
        /// All possible access rights.
        /// </summary>
        GenericAll = 0x10000000,
    }
}