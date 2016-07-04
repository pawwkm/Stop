using System;
using System.IO;
using System.Management;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Selects a disk
    /// </summary>
    internal class SelectDiskCommand : Command
    {
        private bool isPhysical;

        private bool ask;

        private int id;

        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDiskCommand"/> class.
        /// The command will ask the user about which disk to use.
        /// </summary>
        public SelectDiskCommand()
        {
            ask = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDiskCommand"/> class.
        /// </summary>
        /// <param name="path">The path where the image should be stored.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> 
        /// </exception>
        public SelectDiskCommand(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDiskCommand"/> class.
        /// </summary>
        /// <param name="physicalDisk">The number of the physical disk to access.</param>
        public SelectDiskCommand(int physicalDisk)
        {
            id = physicalDisk;
            isPhysical = true;
        }

        /// <summary>
        /// Selects a physical disk or a file.
        /// </summary>
        /// <param name="context">The context of the script.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public override void Execute(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Disk != null)
                context.Disk.Dispose();

            if (ask)
            {
                ListDisks();

                string raw = Console.ReadLine();
                int number;

                if (int.TryParse(raw, out number))
                    context.Disk = new DiskStream(number, true);
                else
                    context.Disk = File.Create(raw);
            }
            else if (isPhysical)
                context.Disk = new DiskStream(id, true);
            else
                context.Disk = File.Create(path);
        }

        /// <summary>
        /// Postfixes a number of bytes with the appropriate unit.
        /// </summary>
        /// <param name="bytes">The number of bytes to postfix.</param>
        /// <returns>The of bytes with a unit.</returns>
        private static string ToSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            for (int i = 0; i < 8; i++)
            {
                var temp = bytes / Math.Pow(1024, i);
                if (temp < 1024)
                    return temp.ToString("#.##") + sizes[i];
            }

            return "";
        }

        /// <summary>
        /// List the currently available physical disks.
        /// </summary>
        private void ListDisks()
        {
            var prefix = "\\\\.\\PHYSICALDRIVE";

            Console.WriteLine("Disk\t\tSize");

            var ms = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject mo in ms.Get())
            {
                var id = mo["DeviceID"].ToString();
                if (id.StartsWith(prefix))
                {
                    var number = id.Substring(prefix.Length);
                    var raw = Convert.ToInt64(mo["Size"]);

                    Console.WriteLine("{0}\t\t{1}", number, ToSize(raw));
                }
            }
        }
    }
}