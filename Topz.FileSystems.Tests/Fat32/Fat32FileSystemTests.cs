using NUnit.Framework;
using System;
using System.IO;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// Provides tests for the <see cref="Fat32FileSystem"/> class.
    /// </summary>
    public class Fat32FileSystemTests
    {
        /// <summary>
        /// Tests taht <see cref="Fat32FileSystem.Create(Stream, Partition, BootSector)"/> creates
        /// a system that can be written and read from by craeting a system, then 
        /// creates a file and read it back.
        /// </summary>
        [Test]
        public void Create_CreateFileSystemAndReadFile_Success()
        {
            Partition[] partitions =
            {
                new Partition(1)
                {
                    Offset = 2048,
                    Sectors = 69632, // 34MB.
                    PartitionType = PartitionType.Fat32,
                    Status = PartitionStatus.Bootable
                },
                new Partition(2),
                new Partition(3),
                new Partition(4)
            };

            MasterBootRecord mbr = new MasterBootRecord(partitions);

            // The minimum number of clusters for Fat32 is 65525.
            const int amountOfClusters = 65525;

            using (MemoryStream disk = new MemoryStream())
            {
                MasterBootRecordSerializer mbrs = new MasterBootRecordSerializer();
                mbrs.Serialize(mbr, disk);

                BootSector boot = new BootSector();
                boot.FatSize = (uint)Math.Ceiling(amountOfClusters / (double)boot.BytesPerSector);
                boot.Sectors = (uint)boot.SectorsPerCluster * amountOfClusters + boot.ReservedSectors + boot.FatSize;

                Fat32FileSystem.Create(disk, mbr.Partitions[0], boot);

                byte[] bytes = new byte[4600]; // Use multiple clusters.
                Random random = new Random();
                random.NextBytes(bytes);

                using (MemoryStream expected = new MemoryStream(bytes))
                {
                    Fat32FileSystem system = new Fat32FileSystem(disk, mbr.Partitions[0]);
                    system.Create("Data.bin");

                    using (Stream file = system.Open("Data.bin"))
                    {
                        expected.CopyTo(file);
                        file.Flush();
                    }

                    using (Stream file = system.Open("Data.bin"))
                        FileAssert.AreEqual(expected, file);
                }
            }
        }
    }
}