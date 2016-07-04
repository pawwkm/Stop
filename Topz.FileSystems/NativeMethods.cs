using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Topz.FileSystems
{
    /// <summary>
    /// Provides access to methods in Kernel32 dll.
    /// </summary>
    internal static class NativeMethods
    {
        private const string Dll = "Kernel32.dll";

        /// <summary>
        /// All possible access rights.
        /// </summary>
        public const uint GenericAll = 0x10000000;

        /// <summary>
        /// The file cannot be shared.
        /// </summary>
        public const uint NoSharing = 0x00000000;

        /// <summary>
        /// Opens a file. The function fails if the file does not exist. 
        /// </summary>
        public const uint OpenExisting = 3;

        /// <summary>
        /// The file does not have other attributes set. 
        /// This attribute is valid only if used alone.
        /// </summary>
        public const uint Normal = 0x00000080;

        public const uint StorageEjectMedia = 2967560;

        public const uint FsctlLockVolume = 589848;

        public const uint FsctlAllowExtendedDasdIo = 589955;

        public const uint FsctlDismountVolume = 589856;

        /// <summary>
        /// Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: 
        /// file, file stream, directory, physical disk, volume, console buffer, tape drive, 
        /// communications resource, mailslot, and pipe. The function returns a handle that can be 
        /// used to access the file or device for various types of I/O depending on the file or 
        /// device and the flags and attributes specified.
        /// </summary>
        /// <param name="lpFileName">
        /// The name of the file or device to be created or opened. You may use either forward slashes
        /// (/) or backslashes (\) in this name. In the ANSI version of this method, the name is limited 
        /// to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode 
        /// version of the function and prepend "\\?\" to the path. 
        /// To create a file stream, specify the name of the file, a colon, and then the name of the stream.
        /// </param>
        /// <param name="dwDesiredAccess">
        /// The requested access to the file or device, which can be summarized as read, write, both or neither zero).
        /// The most commonly used values are GENERIC_READ, GENERIC_WRITE, or both (GENERIC_READ | GENERIC_WRITE).
        /// If this parameter is zero, the application can query certain metadata such as file, directory, or device 
        /// attributes without accessing that file or device, even if GENERIC_READ access would have been denied.
        /// You cannot request an access mode that conflicts with the sharing mode that is specified by the dwShareMode 
        /// parameter in an open request that already has an open handle.
        /// </param>
        /// <param name="dwShareMode">
        /// The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or 
        /// none. Access requests to attributes or extended attributes are not affected by this flag.
        /// If this parameter is zero and CreateFile succeeds, the file or device cannot be shared and cannot be opened
        /// again until the handle to the file or device is closed. For more information, see the Remarks section.
        /// You cannot request a sharing mode that conflicts with the access mode that is specified in an existing request
        /// that has an open handle. CreateFile would fail and the GetLastError function would return ERROR_SHARING_VIOLATION.
        /// </param>
        /// <param name="lpSecurityAttributes">
        /// A pointer to a SECURITY_ATTRIBUTES structure that contains two separate but related data members: an optional security
        /// descriptor, and a Boolean value that determines whether the returned handle can be inherited by child processes.
        /// This parameter can be NULL. If this parameter is NULL, the handle returned by CreateFile cannot be inherited by any child
        /// processes the application may create and the file or device associated with the returned handle gets a default security descriptor.
        /// The lpSecurityDescriptor member of the structure specifies a SECURITY_DESCRIPTOR for a file or device. If this member is NULL, 
        /// the file or device associated with the returned handle is assigned a default security descriptor. CreateFile ignores the 
        /// lpSecurityDescriptor member when opening an existing file or device, but continues to use the bInheritHandle member.
        /// The bInheritHandlemember of the structure specifies whether the returned handle can be inherited.
        /// For more information, see the Remarks section.
        /// </param>
        /// <param name="dwCreationDisposition">
        /// An action to take on a file or device that exists or does not exist.
        /// For devices other than files, this parameter is usually set to OPEN_EXISTING.
        /// For more information, see the Remarks section.
        /// </param>
        /// <param name="dwFlagsAndAttributes">
        /// The file or device attributes and flags, FILE_ATTRIBUTE_NORMAL being the most common default value for files.
        /// This parameter can include any combination of the available file attributes (FILE_ATTRIBUTE_*). All other file attributes 
        /// override FILE_ATTRIBUTE_NORMAL. This parameter can also contain combinations of flags (FILE_FLAG_*) for control of file 
        /// or device caching behavior, access modes, and other special-purpose flags. These combine with any FILE_ATTRIBUTE_* values.
        /// This parameter can also contain Security Quality of Service (SQOS) information by specifying the SECURITY_SQOS_PRESENT flag. 
        /// Additional SQOS-related flags information is presented in the table following the attributes and flags tables.
        /// Note  When CreateFile opens an existing file, it generally combines the file flags with the file attributes of the existing 
        /// file, and ignores any file attributes supplied as part of dwFlagsAndAttributes. 
        /// Some of the following file attributes and flags may only apply to files and not necessarily all other types of devices that
        /// CreateFile can open. For additional information, see the Remarks section of this topic 
        /// </param>
        /// <param name="hTemplateFile">
        /// A valid handle to a template file with the GENERIC_READ access right. The template file supplies file attributes and extended 
        /// attributes for the file that is being created. This parameter can be NULL. When opening an existing file, CreateFile ignores this parameter.
        /// When opening a new encrypted file, the file inherits the discretionary access control list from its parent directory. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.
        /// </returns>
        [DllImport(Dll, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport(Dll, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(SafeFileHandle handle);

        [DllImport(Dll, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint IoControlCode, [MarshalAs(UnmanagedType.AsAny)][In] object InBuffer, uint nInBufferSize, [MarshalAs(UnmanagedType.AsAny)] [Out] object OutBuffer, uint nOutBufferSize, ref uint pBytesReturned, [In] IntPtr Overlapped);

        [DllImport(Dll, SetLastError = true, CharSet = CharSet.Auto)]
        [SuppressMessage("Microsoft.Interoperability", "CA1415:DeclarePInvokesCorrectly", Justification = Justifications.ItWorksNoTouchy)]
        public static extern unsafe bool ReadFile(SafeFileHandle hFile, byte* pBuffer, uint NumberOfBytesToRead, uint* pNumberOfBytesRead, IntPtr Overlapped);

        [DllImport(Dll, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetFilePointerEx(SafeFileHandle hFile, ulong liDistanceToMove, out ulong lpNewFilePointer, uint dwMoveMethod);

        [DllImport(Dll, SetLastError = true, CharSet = CharSet.Auto)]
        [SuppressMessage("Microsoft.Interoperability", "CA1415:DeclarePInvokesCorrectly", Justification = Justifications.ItWorksNoTouchy)]
        public static extern unsafe bool WriteFile(SafeFileHandle hFile, byte* pBuffer, uint NumberOfBytesToWrite, uint* pNumberOfBytesWritten, IntPtr Overlapped);
    }
}