using System;
using System.IO;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// Serializes <see cref="FileSystemInfo"/>.
    /// </summary>
    public class FileSystemInfoSerializer : ISerializer<FileSystemInfo>
    {
        /// <summary>
        /// Deserializes a piece of data from a stream. 
        /// </summary>
        /// <param name="stream">The stream to deserialize the data from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public FileSystemInfo Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            FileSystemInfo info = new FileSystemInfo();

            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            info.LeadSignature = BitConverter.ToUInt32(bytes, 0);

            // Skip reserved.
            stream.Position += 480;

            stream.Read(bytes, 0, bytes.Length);
            info.StructSignature = BitConverter.ToUInt32(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            info.LastFreeCluster = BitConverter.ToUInt32(bytes, 0);

            stream.Read(bytes, 0, bytes.Length);
            info.NextFreeCluster = BitConverter.ToUInt32(bytes, 0);

            // Skip reserved.
            stream.Position += 12;

            stream.Read(bytes, 0, bytes.Length);
            info.TrailSignature = BitConverter.ToUInt32(bytes, 0);

            return info;
        }

        /// <summary>
        /// Serializes a piece of data to a stream.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="stream">The stream the serialized data is stored in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> or <paramref name="stream"/> is null.
        /// </exception>
        public void Serialize(FileSystemInfo data, Stream stream)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(BitConverter.GetBytes(data.LeadSignature), 0, 4);

            // Skip reserved.
            stream.Position += 480;

            stream.Write(BitConverter.GetBytes(data.StructSignature), 0, 4);
            stream.Write(BitConverter.GetBytes(data.LastFreeCluster), 0, 4);
            stream.Write(BitConverter.GetBytes(data.NextFreeCluster), 0, 4);

            // Skip reserved.
            stream.Position += 12;

            stream.Write(BitConverter.GetBytes(data.TrailSignature), 0, 4);
        }
    }
}