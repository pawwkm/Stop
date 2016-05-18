using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace Stop.FileSystems
{
    /// <summary>
    /// Streams the raw contents of a disk.
    /// </summary>
    public class DiskStream : Stream
    {
        private SafeFileHandle handle;

        private long length;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskStream"/> class.
        /// </summary>
        /// <param name="physicalDrive">The number of the physical drive to read from.</param>
        /// <exception cref="ArgumentException">
        /// There is no physical drive with the given number.
        /// </exception>
        /// <exception cref="COMException">
        /// An error happend in the WinApi.
        /// </exception>
        public DiskStream(int physicalDrive)
        {
            var name = GetDriveId(physicalDrive);
            if (name == null)
                throw new ArgumentException("There is no physical drive with the given number.", nameof(physicalDrive));

            UnmountVolumes(physicalDrive);

            handle = Kernel32.CreateFile(name, EFileAccess.GenericAll, EFileShare.None, IntPtr.Zero, ECreationDisposition.OpenExisting, EFileAttributes.Normal, IntPtr.Zero);
            Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

            length = GetLength(physicalDrive);
        }

        /// <summary>
        /// The stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The stream supports reading.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The length of the disk in bytes.
        /// </summary>
        public override long Length
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// The current position within the stream.
        /// </summary>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// This doesn't do anything. Data is automatically flushed.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and 
        /// advances the position within the stream by the number 
        /// of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// When this method returns, the buffer contains the specified
        /// byte array with the values between offset and 
        /// (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by
        /// the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// Offset in buffer at which to begin storing the data read 
        /// from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number
        /// of bytes requested if that many bytes are not currently available, or zero (0)
        /// if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            long pos = Position;
            int allignmentOffset = (int)(pos % 512);
            int sectorsToRead = (int)Math.Ceiling((count + allignmentOffset) / 512f);

            byte[] bytes = new byte[sectorsToRead * 512];

            uint n = 0;
            unsafe
            {   
                fixed (byte* p = bytes)
                {
                    if (!Kernel32.ReadFile(handle, p, (uint)bytes.Length, &n, IntPtr.Zero))
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }

            Buffer.BlockCopy(bytes, allignmentOffset, buffer, offset, count);

            Position = pos + count;

            return (int)n;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/>.</param>
        /// <param name="origin">The reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            ulong n = 0;
            if (!Kernel32.SetFilePointerEx(handle, (ulong)offset, out n, (uint)origin))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            return (long)n;
        }

        /// <summary>
        /// The physical disk can't be resized and is 
        /// therefore not supported.
        /// </summary>
        /// <param name="value">This value is not used.</param>
        /// <exception cref="NotSupportedException">
        /// This is not supported.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current 
        /// stream and advances the current position within this 
        /// stream by the number of bytes written
        /// </summary>
        /// <param name="buffer">The bytes to write.</param>
        /// <param name="offset">
        /// Byte offset in <paramref name="buffer"/> at which to begin 
        /// copying bytes to the current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            unsafe
            {
                uint n = 0;
                fixed (byte* p = buffer)
                {
                    if (!Kernel32.WriteFile(handle, p + offset, (uint)count, &n, IntPtr.Zero))
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the stream and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to 
        /// release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (handle != null)
            {
                Kernel32.CloseHandle(handle);
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                handle.SetHandleAsInvalid();
                handle = null;
            }

            // This should mount the unmounted drives.
            DriveInfo.GetDrives();
        }

        /// <summary>
        /// Gets the id for a physical disk.
        /// </summary>
        /// <param name="number">The number of the disk.</param>
        /// <returns>The id for the disk if it exists; otherwise null.</returns>
        private static string GetDriveId(int number)
        {
            var exist = false;
            var deviceId = string.Format(@"DeviceID=""\\\\.\\PHYSICALDRIVE{0}""", number);

            var ms = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject mo in ms.Get())
            {
                if (mo.Path.Path.EndsWith(deviceId))
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
                return null;

            return @"\\.\PHYSICALDRIVE" + number;
        }

        /// <summary>
        /// Gets the length of a physical disk.
        /// </summary>
        /// <param name="number">The number of the disk.</param>
        /// <returns>The length of the disk.</returns>
        private static long GetLength(int number)
        {
            var deviceId = string.Format(@"DeviceID=""\\\\.\\PHYSICALDRIVE{0}""", number);

            var ms = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject mo in ms.Get())
            {
                if (mo.Path.Path.EndsWith(deviceId))
                    return Convert.ToInt64(mo["Size"]);
            }

            return 0;
        }

        /// <summary>
        /// Unmounts all the drives on a physical disk.
        /// </summary>
        /// <param name="number">The number of the physical disk.</param>
        private static void UnmountVolumes(int number)
        {
            foreach (var letter in GetDriveLettersFrom(number))
            {
                var drive = "\\\\.\\" + letter + ':';

                var handle = Kernel32.CreateFile(drive, EFileAccess.GenericAll, EFileShare.None, IntPtr.Zero, ECreationDisposition.OpenExisting, EFileAttributes.Normal, IntPtr.Zero);
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                uint bytesReturned = 0;
                Kernel32.DeviceIoControl(handle, EIOControlCode.FsctlDismountVolume, null, 0, null, 0, ref bytesReturned, IntPtr.Zero);
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                Kernel32.CloseHandle(handle);
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Gets the drive letters from a physical drive.
        /// </summary>
        /// <param name="physical">The number of the physical drive.</param>
        /// <returns>A list of the drive letters of the physical drive.</returns>
        private static IEnumerable<char> GetDriveLettersFrom(int physical)
        {
            var diskSearcher = new ManagementObjectSearcher("root\\CIMV2", "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + @"\\.\PHYSICALDRIVE" + physical + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
            foreach (ManagementObject mo in diskSearcher.Get())
            {
                var driveSearcher = new ManagementObjectSearcher("root\\CIMV2", "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + mo["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition");
                foreach (ManagementObject driveMo in driveSearcher.Get())
                    yield return driveMo["DeviceID"].ToString()[0];
            }
        }
    }
}