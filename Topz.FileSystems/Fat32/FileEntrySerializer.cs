using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// Serializes <see cref="FileEntry"/>.
    /// </summary>
    public class FileEntrySerializer : ISerializer<FileEntry>
    {
        /// <summary>
        /// Deserializes a piece of data from a stream. 
        /// </summary>
        /// <param name="stream">The stream to deserialize the data from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public FileEntry Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            byte[] bytes = new byte[11];
            stream.Read(bytes, 0, bytes.Length);

            FileEntry entry = new FileEntry(bytes);
            entry.Attributes = (FileAttributes)stream.ReadByte();

            // Skip reserved.
            stream.Position++;

            entry.CreationTimeMilliseconds = (byte)stream.ReadByte();

            bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            entry.CreationTime = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            entry.CreationDate = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            entry.LastAccessDate = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            entry.FirstClusterHigh = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            entry.WriteTime = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            entry.WriteDate = BitConverter.ToUInt16(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            entry.FirstClusterLow = BitConverter.ToUInt16(bytes, 0);

            bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            entry.FileSize = BitConverter.ToUInt32(bytes, 0);

            return entry;
        }

        /// <summary>
        /// Serializes a piece of data to a stream.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="stream">The stream the serialized data is stored in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> or <paramref name="stream"/> is null.
        /// </exception>
        public void Serialize(FileEntry data, Stream stream)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(Encoding.ASCII.GetBytes(data.ShortName), 0, 11);
            stream.WriteByte((byte)data.Attributes);

            stream.Position += 1;

            stream.WriteByte(data.CreationTimeMilliseconds);

            stream.Write(BitConverter.GetBytes(data.CreationTime), 0, 2);
            stream.Write(BitConverter.GetBytes(data.CreationDate), 0, 2);
            stream.Write(BitConverter.GetBytes(data.LastAccessDate), 0, 2);
            stream.Write(BitConverter.GetBytes(data.FirstClusterHigh), 0, 2);
            stream.Write(BitConverter.GetBytes(data.WriteTime), 0, 2);
            stream.Write(BitConverter.GetBytes(data.WriteDate), 0, 2);
            stream.Write(BitConverter.GetBytes(data.FirstClusterLow), 0, 2);
            stream.Write(BitConverter.GetBytes(data.FileSize), 0, 4);
        }
    }
}