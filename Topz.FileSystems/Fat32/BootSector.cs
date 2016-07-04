using Pote;
using System;
using System.Linq;
using System.Text;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// This is the first sector of a Fat32.
    /// </summary>
    [Serializer(typeof(BootSectorSerializer))]
    public class BootSector
    {
        private const string Fat12 = "FAT12   ";

        private const string Fat16 = "FAT16   ";

        private const string Fat32 = "FAT32   ";
        
        private byte[] oemName = new byte[8];

        private ushort bytesPerSector = 512;

        private byte sectorsPerCluster = 1;

        private ushort reservedSectors = 1;

        private byte fats = 1;

        private ushort sectorsPerTrack;

        private ushort heads;

        private uint hiddenSectors;

        private uint sectors = 1;

        private uint fatSize;

        private ushort flags;

        private ushort version;

        private uint rootCluster = 2;

        private ushort fileSystemInfoSector = 1;

        private ushort backupBootSector;

        private byte driveNumber;

        private byte bootSignature;

        private uint id = (uint)DateTime.Now.Ticks;

        private byte[] label = new byte[11];
        
        private byte[] systemType = new byte[8];

        /// <summary>
        /// Initializes a new instance of the <see cref="BootSector"/> class.
        /// </summary>
        public BootSector()
        {
            Label = "Disk";
            OemName = "MSWIN4.1";
        }

        /// <summary>
        /// This is only a name string that nobody really pays any attention to.
        /// However some drivers will not recognize the system if its OEM name 
        /// is not "MSWIN4.1". This name is also an indication of what system
        /// formatted the disk.
        /// </summary>
        public string OemName
        {
            get
            {
                int index = Array.IndexOf<byte>(oemName, 0);
                if (index == -1)
                    return Encoding.ASCII.GetString(oemName);

                return Encoding.ASCII.GetString(oemName.Take(index).ToArray());
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var name = Encoding.ASCII.GetBytes(value);
                if (name.Length > 8)
                    throw new ArgumentOutOfRangeException(nameof(value), "The OEM name must be 8 characters or less.");

                Buffer.BlockCopy(name, 0, oemName, 0, name.Length);
                for (int i = name.Length; i < 8; i++)
                    oemName[i] = 0x00;
            }
        }

        /// <summary>
        /// The number of bytes per sector. Some older drivers 
        /// just assume this is 512, if you want to ensure maximum 
        /// compatibility this should be 512.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not 512, 1024, 2048 or 4096.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="SectorsPerCluster"/> * <see cref="BytesPerSector"/> &gt; 32K.
        /// </exception>
        public ushort BytesPerSector
        {
            get
            {
                return bytesPerSector;
            }
            set
            {
                if (!value.IsOneOf(512, 1024, 2048, 4096))
                    throw new ArgumentOutOfRangeException(nameof(value), "It must be 512, 1024, 2048 or 4096 bytes per sector.");
                if (value * SectorsPerCluster > 32768)
                    throw new ArgumentException(nameof(SectorsPerCluster) + " * " + nameof(BytesPerSector) + " > 32k", nameof(value));

                bytesPerSector = value;
            }
        }

        /// <summary>
        /// The number of sectors per cluster. This must be 1, 2, 4, 8, 16, 32, 64 or 128.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not 1, 2, 4, 8, 16, 32, 64 or 128 sectors per cluster.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="SectorsPerCluster"/> * <see cref="BytesPerSector"/> &gt; 32K.
        /// </exception>
        public byte SectorsPerCluster
        {
            get
            {
                return sectorsPerCluster;
            }
            set
            {
                if (!value.IsOneOf(1, 2, 4, 8, 16, 32, 64, 128))
                    throw new ArgumentOutOfRangeException(nameof(value), "It must be 1, 2, 4, 8, 16, 32, 64 or 128 sectors per cluster.");
                if (value * BytesPerSector > 32768)
                    throw new ArgumentException(nameof(SectorsPerCluster) + " * " + nameof(BytesPerSector) + " > 32k", nameof(value));

                sectorsPerCluster = value;
            }
        }

        /// <summary>
        /// Number of reserved sectors in the Reserved region of 
        /// the volume starting at the first sector of the volume.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is zero.
        /// </exception>
        public ushort ReservedSectors
        {
            get
            {
                return reservedSectors;
            }
            set
            {
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "This must not be zero.");

                reservedSectors = value;
            }
        }

        /// <summary>
        /// The number of fat structures on the disk.
        /// Some operating systems may not recognize the 
        /// disk if this number is not 2.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is zero.
        /// </exception>
        public byte Fats
        {
            get
            {
                return fats;
            }
            set
            {
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "This must not be zero.");

                fats = value;
            }
        }

        /// <summary>
        /// The number of sectors per track. This is only relevant for
        /// media that has geometry.
        /// </summary>
        public ushort SectorsPerTrack
        {
            get
            {
                return sectorsPerTrack;
            }
            set
            {
                sectorsPerTrack = value;
            }
        }

        /// <summary>
        /// The number of heads on the media. This is only relevant for
        /// media that has geometry.
        /// </summary>
        public ushort Heads
        {
            get
            {
                return heads;
            }
            set
            {
                heads = value;
            }
        }

        /// <summary>
        /// Number of hidden sectors preceding the partition that contains this
        /// FAT volume.This field is generally only relevant for media visible
        /// on interrupt 0x13. This field should always be zero on media that
        /// are not partitioned. Exactly what value is appropriate is operating
        /// system specific.
        /// </summary>
        public uint HiddenSectors
        {
            get
            {
                return hiddenSectors;
            }
            set
            {
                hiddenSectors = value;
            }
        }

        /// <summary>
        /// Number of sectors on the volume. This count includes the 
        /// count of all sectors in all four regions of the volume. 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is zero.
        /// </exception>
        public uint Sectors
        {
            get
            {
                return sectors;
            }
            set
            {
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "This must not be zero.");

                sectors = value;
            }
        }

        /// <summary>
        /// The number of sectors used by one FAT.
        /// </summary>
        public uint FatSize
        {
            get
            {
                return fatSize;
            }
            set
            {
                fatSize = value;
            }
        }

        /// <summary>
        /// Bits 0-3   -- Zero-based number of active FAT. Only valid if mirroring is disabled.
        /// Bits 4-6   -- Reserved.
        /// Bit    7   -- 0 means the FAT is mirrored at runtime into all FATs.
        ///            -- 1 means only one FAT is active; it is the one referenced in bits 0-3.
        /// Bits 8-15  -- Reserved.
        /// </summary>
        public ushort SystemAttributes
        {
            get
            {
                return flags;
            }
            set
            {
                flags = value;
            }
        }

        /// <summary>
        /// This is the version number of FAT32 volume.
        /// High byte is major revision number. Low byte is minor 
        /// revision number. This should be 0:0.
        /// </summary>
        public ushort Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        /// <summary>
        /// This is set to the cluster number of the first cluster 
        /// of the root directory, usually 2 but not required to be 2. 
        /// </summary>
        public uint RootCluster
        {
            get
            {
                return rootCluster;
            }
            set
            {
                rootCluster = value;
            }
        }

        /// <summary>
        /// Sector number of the file system info structure
        /// in the reserved area of the FAT. Usually 1. There
        /// will be a copy of the structure in the <see cref="BackupBootSector"/>,
        /// but only the copy pointed to by this field will be kept up to date
        /// (i.e. both the primary and backup boot record will point to the same
        /// file system info structure sector).
        /// </summary>
        public ushort FileSystemInfoSector
        {
            get
            {
                return fileSystemInfoSector;
            }
            set
            {
                fileSystemInfoSector = value;
            }
        }

        /// <summary>
        /// If non-zero, indicates the sector number in the reserved 
        /// area of the volume of a copy of the boot record.
        /// Usually 6. No value other than 6 is recommended.
        /// </summary>
        public ushort BackupBootSector
        {
            get
            {
                return backupBootSector;
            }
            set
            {
                backupBootSector = value;
            }
        }

        /// <summary>
        /// Int 0x13 drive number (e.g. 0x80). This field supports MS-DOS bootstrap 
        /// and is set to the INT 0x13 drive number of the media (0x00 for floppy disks, 0x80 for hard disks).
        /// This field is actually operating system specific. 
        /// </summary>
        public byte DriveNumber
        {
            get
            {
                return driveNumber;
            }
            set
            {
                driveNumber = value;
            }
        }

        /// <summary>
        /// Extended boot signature (0x29). This is a signature byte that indicates that the following three
        /// properties <see cref="Id"/>, <see cref="Label"/> and <see cref="SystemType"/> are present in the
        /// boot sector.
        /// </summary>
        public byte BootSignature
        {
            get
            {
                return bootSignature;
            }
            set
            {
                bootSignature = value;
            }
        }

        /// <summary>
        /// Volume serial number. This field, together with <see cref="Label"/>, 
        /// supports volume tracking on removable media.These values allow 
        /// FAT file system drivers to detect that the wrong disk is inserted in a 
        /// removable drive.This ID is usually generated by simply combining
        /// the current date and time into a 32-bit value. 
        /// </summary>
        public uint Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Volume label. This field matches the 11-byte volume label 
        /// recorded in the root directory. FAT file system drivers 
        /// should make sure that they update this field when the volume 
        /// label file in the root directory has its name changed or 
        /// created.The setting for this field when there is no
        /// volume label is the string "NO NAME    ".
        /// </summary>
        public string Label
        {
            get
            {
                int index = Array.IndexOf<byte>(label, 0);
                if (index == -1)
                    return Encoding.ASCII.GetString(label);

                return Encoding.ASCII.GetString(label.Take(index).ToArray());
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var name = Encoding.ASCII.GetBytes(value);
                if (name.Length > 11)
                    throw new ArgumentOutOfRangeException(nameof(value), "The lable must be 11 characters or less.");

                Buffer.BlockCopy(name, 0, label, 0, name.Length);
                for (int i = name.Length; i < 11; i++)
                    label[i] = 0x00;
            }
        }

        /// <summary>
        /// The name of the file system type. This can't however be used 
        /// for determining which type it really is.
        /// </summary>
        public SystemType SystemType
        {
            get
            {
                string type = Encoding.ASCII.GetString(systemType);
                switch (type)
                {
                    case Fat12:
                        return SystemType.Fat12;
                    case Fat16:
                        return SystemType.Fat16;
                    default:
                        return SystemType.Fat32;
                }
            }
            set
            {
                switch (value)
                {
                    case SystemType.Fat12:
                        systemType = Encoding.ASCII.GetBytes(Fat12);
                        break;
                    case SystemType.Fat16:
                        systemType = Encoding.ASCII.GetBytes(Fat16);
                        break;
                    default:
                        systemType = Encoding.ASCII.GetBytes(Fat32);
                        break;
                }
            }
        }

        /// <summary>
        /// The number of bytes in a cluster.
        /// </summary>
        public uint BytesPerCluster
        {
            get
            {
                return (uint)(SectorsPerCluster * BytesPerSector);
            }
        }

        /// <summary>
        /// The number of clusters in the FAT.
        /// </summary>
        public uint Clusters
        {
            get
            {
                return FatSize * BytesPerSector / 4;
            }
        }
    }
}