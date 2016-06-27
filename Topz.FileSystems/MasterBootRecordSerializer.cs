using System;
using System.IO;

namespace Topz.FileSystems
{
    /// <summary>
    /// Serializes <see cref="MasterBootRecord"/>.
    /// </summary>
    public class MasterBootRecordSerializer : ISerializer<MasterBootRecord>
    {
        /// <summary>
        /// Deserializes a piece of data from a stream. 
        /// </summary>
        /// <param name="stream">The stream to deserialize the data from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public MasterBootRecord Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Position = 446;

            Partition[] partitions = new Partition[4];
            for (int i = 0; i < 4; i++)
            {
                partitions[i] = new Partition(i + 1);

                int status = stream.ReadByte();
                if (status == 0x80)
                    partitions[i].Status = PartitionStatus.Bootable;
                else if (status == 0)
                    partitions[i].Status = PartitionStatus.Inactive;
                else
                    partitions[i].Status = PartitionStatus.Invalid;

                stream.Position += 3;
                partitions[i].PartitionType = (PartitionType)stream.ReadByte();
                stream.Position += 3;

                byte[] bytes = new byte[4];
                stream.Read(bytes, 0, bytes.Length);
                partitions[i].Offset = BitConverter.ToUInt32(bytes, 0);

                stream.Read(bytes, 0, bytes.Length);
                partitions[i].Sectors = BitConverter.ToUInt32(bytes, 0);
            }

            return new MasterBootRecord(partitions);
        }

        /// <summary>
        /// Serializes a piece of data to a stream.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="stream">The stream the serialized data is stored in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> or <paramref name="stream"/> is null.
        /// </exception>
        public void Serialize(MasterBootRecord data, Stream stream)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Position = 446;
            foreach (Partition partition in data.Partitions)
            {
                if (partition.Status == PartitionStatus.Bootable)
                    stream.WriteByte(0x80);
                else if (partition.Status == PartitionStatus.Inactive)
                    stream.WriteByte(0x00);
                else
                    stream.WriteByte(0xBF);

                // Max out the start CHS.
                stream.WriteByte(0xFF);
                stream.WriteByte(0xFF);
                stream.WriteByte(0xFF);

                stream.WriteByte((byte)partition.PartitionType);

                // Max out the end CHS.
                stream.WriteByte(0xFF);
                stream.WriteByte(0xFF);
                stream.WriteByte(0xFF);

                byte[] bytes = BitConverter.GetBytes(partition.Offset);
                stream.Write(bytes, 0, bytes.Length);

                bytes = BitConverter.GetBytes(partition.Sectors);
                stream.Write(bytes, 0, bytes.Length);
            }

            // Boot signature.
            stream.WriteByte(0x55);
            stream.WriteByte(0xAA);
        }

        private uint ChsToLba(byte[] chs)
        {
            ushort cylinderSector = BitConverter.ToUInt16(chs, 1);

            byte heads = chs[0];
            ushort sector = (ushort)(cylinderSector & 0x3F);
            ushort cylinder = (ushort)(cylinderSector & 0xFFC0);

            // The geometry is needed here.

            return 0;
        }
    }
}