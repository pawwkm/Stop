using System;
using System.Runtime.InteropServices;

namespace Stop.FileSystems
{
    /// <summary>
    /// Provides methods for manipulating structures,
    /// </summary>
    internal static class Structures
    {
        /// <summary>
        /// Converts a structure to bytes.
        /// </summary>
        /// <typeparam name="T">The type of structure to convert.</typeparam>
        /// <param name="structure">The instance to convert.</param>
        /// <returns>The converted structure as a sequence of bytes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="structure"/> is null.
        /// </exception>
        public static byte[] ToBytes<T>(T structure)
        {
            if (structure == null)
                throw new ArgumentNullException("structure");

            IntPtr ptr = IntPtr.Zero;
            try
            {
                int length = Marshal.SizeOf(structure);
                ptr = Marshal.AllocHGlobal(length);
                byte[] buffer = new byte[length];

                Marshal.StructureToPtr(structure, ptr, true);
                Marshal.Copy(ptr, buffer, 0, length);
                Marshal.FreeHGlobal(ptr);

                return buffer;
            }
            catch
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);

                throw;
            }
        }
    }
}