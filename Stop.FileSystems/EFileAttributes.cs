using System;

namespace Stop.FileSystems
{
    /// <summary>
    /// The file or device attributes and flags.
    /// </summary>
    [Flags]
    public enum EFileAttributes : uint
    {
        /// <summary>
        /// The file is read only. Applications can read the file, 
        /// but cannot write to or delete it.The file is read only. 
        /// Applications can read the file, but cannot write to or 
        /// delete it.
        /// </summary>
        Readonly = 0x00000001,

        /// <summary>
        /// The file is hidden. Do not include it in
        /// an ordinary directory listing.
        /// </summary>
        Hidden = 0x00000002,

        /// <summary>
        /// The file is part of or used exclusively 
        /// by an operating system.
        /// </summary>
        System = 0x00000004,
        
        /// <summary>
        /// The file should be archived. Applications use this attribute to 
        /// mark files for backup or removal.
        /// </summary>
        Archive = 0x00000020,

        /// <summary>
        /// The file does not have other attributes set. 
        /// This attribute is valid only if used alone.
        /// </summary>
        Normal = 0x00000080,

        /// <summary>
        /// The file is being used for temporary storage.
        /// </summary>
        Temporary = 0x00000100,

        /// <summary>
        /// The data of a file is not immediately available. 
        /// This attribute indicates that file data is physically 
        /// moved to offline storage. This attribute is used by 
        /// Remote Storage, the hierarchical storage management 
        /// software. Applications should not arbitrarily change 
        /// this attribute.
        /// </summary>
        Offline = 0x00001000,

        /// <summary>
        /// The file or directory is encrypted. For a file, this means that 
        /// all data in the file is encrypted. For a directory, this means 
        /// that encryption is the default for newly created files and sub 
        /// directories. This flag has no effect if <see cref="EFileAttributes.System"/>
        /// is also specified.
        /// </summary>
        Encrypted = 0x00004000,
        
        /// <summary>
        /// Write operations will not go through any intermediate 
        /// cache, they will go directly to disk.
        /// </summary>
        WriteThrough = 0x80000000,
        
        /// <summary>
        /// The file or device is being opened or created 
        /// for asynchronous I/O. When subsequent I/O operations
        /// are completed on this handle, the event specified
        /// in the OVERLAPPED structure will be set to the signaled
        /// state. If this flag is specified, the file can be used for
        /// simultaneous read and write operations. If this flag is not
        /// specified, then I/O operations are serialized, even if the 
        /// calls to the read and write functions specify an OVERLAPPED
        /// structure.
        /// </summary>
        Overlapped = 0x40000000,

        /// <summary>
        /// The file or device is being opened with no 
        /// system caching for data reads and writes.
        /// This flag does not affect hard disk caching 
        /// or memory mapped files. 
        /// There are strict requirements for successfully 
        /// working with files opened with CreateFile using 
        /// the FILE_FLAG_NO_BUFFERING flag.
        /// </summary>
        NoBuffering = 0x20000000,

        /// <summary>
        /// Access is intended to be random. The system can use this as a hint to optimize file caching.
        /// This flag has no effect if the file system does not support cached I/O and <see cref="NoBuffering"/>.
        /// </summary>
        RandomAccess = 0x10000000,

        /// <summary>
        /// Access is intended to be sequential from beginning
        /// to end. The system can use this as a hint to optimize
        /// file caching. This flag should not be used if read-behind 
        /// (that is, reverse scans) will be used. This flag has no effect 
        /// if the file system does not support cached 
        /// I/O and <see cref="NoBuffering"/>.
        /// </summary>
        SequentialScan = 0x08000000,

        /// <summary>
        /// The file is to be deleted immediately after all 
        /// of its handles are closed, which includes the 
        /// specified handle and any other open or 
        /// duplicated handles.
        /// If there are existing open handles to a file, the 
        /// call fails unless they were all opened with the 
        /// FILE_SHARE_DELETE share mode.
        /// Subsequent open requests for the file fail, unless 
        /// the FILE_SHARE_DELETE share mode is specified.
        /// </summary>
        DeleteOnClose = 0x04000000,

        /// <summary>
        /// The file is being opened or created for a backup or 
        /// restore operation. The system ensures that the calling
        /// process overrides file security checks when the process 
        /// has SE_BACKUP_NAME and SE_RESTORE_NAME privileges.
        /// You must set this flag to obtain a handle to a directory. 
        /// A directory handle can be passed to some functions instead
        /// of a file handle
        /// </summary>
        BackupSemantics = 0x02000000,

        /// <summary>
        /// Access will occur according to POSIX rules. 
        /// This includes allowing multiple files with 
        /// names, differing only in case, for file systems 
        /// that support that naming. Use care when using 
        /// this option, because files created with this 
        /// flag may not be accessible by applications 
        /// that are written for MS-DOS or 16-bit Windows.
        /// </summary>
        PosixSemantics = 0x01000000,

        /// <summary>
        /// Normal reparse point processing will not occur; 
        /// CreateFile will attempt to open the reparse point.
        /// When a file is opened, a file handle is returned,
        /// whether or not the filter that controls the reparse 
        /// point is operational. This flag cannot be used with 
        /// the CREATE_ALWAYS flag. If the file is not a reparse 
        /// point, then this flag is ignored.
        /// </summary>
        OpenReparsePoint = 0x00200000,

        /// <summary>
        /// The file data is requested, but it should continue 
        /// to be located in remote storage. It should not be 
        /// transported back to local storage. This flag is 
        /// for use by remote storage systems.
        /// </summary>
        OpenNoRecall = 0x00100000
    }
}