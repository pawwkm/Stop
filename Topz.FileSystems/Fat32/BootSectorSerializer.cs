using System;
using System.IO;
using System.Text;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// Serializes <see cref="BootSector"/>.
    /// </summary>
    public class BootSectorSerializer : ISerializer<BootSector>
    {
        /// <summary>
        /// Deserializes a piece of data from a stream. 
        /// </summary>
        /// <param name="stream">The stream to deserialize the data from.</param>
        /// <returns>The deserialized data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public BootSector Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            BootSector boot = new BootSector();

            // Skip jump code.
            stream.Position += 3;

            byte[] bytes = new byte[8];
            stream.Read(bytes, 0, bytes.Length);
            boot.OemName = Encoding.ASCII.GetString(bytes);

            bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            boot.BytesPerSector = BitConverter.ToUInt16(bytes, 0);

            boot.SectorsPerCluster = (byte)stream.ReadByte();

            stream.Read(bytes, 0, bytes.Length);
            boot.ReservedSectors = BitConverter.ToUInt16(bytes, 0);

            boot.Fats = (byte)stream.ReadByte();

            // Skip root entries, sectors16, media and fatSize16.
            stream.Position += 7;

            stream.Read(bytes, 0, bytes.Length);
            boot.SectorsPerTrack = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            boot.Heads = BitConverter.ToUInt16(bytes, 0);

            bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            boot.HiddenSectors = BitConverter.ToUInt32(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            boot.Sectors = BitConverter.ToUInt32(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            boot.FatSize = BitConverter.ToUInt32(bytes, 0);

            bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            boot.Flags = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            boot.Version = BitConverter.ToUInt16(bytes, 0);

            bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            boot.RootCluster = BitConverter.ToUInt32(bytes, 0);

            bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            boot.FileSystemInfoSector = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            boot.BackupBootSector = BitConverter.ToUInt16(bytes, 0);

            // Skip reserved.
            stream.Position += 12;

            boot.DriveNumber = (byte)stream.ReadByte();

            // Skip reserved.
            stream.Position++;

            boot.BootSignature = (byte)stream.ReadByte();

            bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            boot.Id = BitConverter.ToUInt32(bytes, 0);

            bytes = new byte[11];
            stream.Read(bytes, 0, bytes.Length);
            boot.Label = Encoding.ASCII.GetString(bytes);

            bytes = new byte[8];
            stream.Read(bytes, 0, bytes.Length);

            switch (Encoding.ASCII.GetString(bytes))
            {
                case "FAT12   ":
                    boot.SystemType = SystemType.Fat12;
                    break;
                case "FAT16   ":
                    boot.SystemType = SystemType.Fat16;
                    break;
                case "FAT32   ":
                    boot.SystemType = SystemType.Fat32;
                    break;
            }

            return boot;
        }

        /// <summary>
        /// Serializes a piece of data to a stream.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="stream">The stream the serialized data is stored in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> or <paramref name="stream"/> is null.
        /// </exception>
        public void Serialize(BootSector data, Stream stream)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // Jump code.
            byte[] bytes = { 0xEB, 0x58, 0x90 };
            stream.Write(bytes, 0, bytes.Length);

            stream.Write(Encoding.ASCII.GetBytes(data.OemName), 0, 8);
            stream.Write(BitConverter.GetBytes(data.BytesPerSector), 0, 2);
            stream.WriteByte(data.SectorsPerCluster);
            stream.Write(BitConverter.GetBytes(data.ReservedSectors), 0, 2);
            stream.WriteByte(data.Fats);

            // Skip root entries and sectors16.
            stream.Position += 4;

            // Hard disk media type.
            stream.WriteByte(0xF8); 

            // Skip fatSize16.
            stream.Position += 2;

            stream.Write(BitConverter.GetBytes(data.SectorsPerTrack), 0, 2);
            stream.Write(BitConverter.GetBytes(data.Heads), 0, 2);
            stream.Write(BitConverter.GetBytes(data.HiddenSectors), 0, 4);
            stream.Write(BitConverter.GetBytes(data.Sectors), 0, 4);
            stream.Write(BitConverter.GetBytes(data.FatSize), 0, 4);
            stream.Write(BitConverter.GetBytes(data.Flags), 0, 2);
            stream.Write(BitConverter.GetBytes(data.Version), 0, 2);
            stream.Write(BitConverter.GetBytes(data.RootCluster), 0, 4);
            stream.Write(BitConverter.GetBytes(data.FileSystemInfoSector), 0, 2);
            stream.Write(BitConverter.GetBytes(data.BackupBootSector), 0, 2);

            // Skip reserved.
            stream.Position += 12;

            stream.WriteByte(data.DriveNumber);

            // Skip reserved.
            stream.Position++;

            stream.WriteByte(data.BootSignature);
            stream.Write(BitConverter.GetBytes(data.Id), 0, 4);

            stream.Write(Encoding.ASCII.GetBytes(data.Label.PadRight(11, '\0')), 0, 11);

            if (data.SystemType == SystemType.Fat12)
                stream.Write(Encoding.ASCII.GetBytes("FAT12   "), 0, 8);
            else if (data.SystemType == SystemType.Fat16)
                stream.Write(Encoding.ASCII.GetBytes("FAT16   "), 0, 8);
            else
                stream.Write(Encoding.ASCII.GetBytes("FAT32   "), 0, 8);
        }
    }
}