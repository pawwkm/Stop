using System;
using System.IO;

namespace Topz.FileSystems
{
    /// <summary>
    /// A serializer for a particular class.
    /// </summary>
    /// <typeparam name="T">
    /// The class to serialize and deserialize.
    /// </typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Serializes a piece of data to a stream.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="stream">The stream the serialized data is stored in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> or <paramref name="stream"/> is null.
        /// </exception>
        void Serialize(T data, Stream stream);

        /// <summary>
        /// Deserializes a piece of data from a stream. 
        /// </summary>
        /// <param name="stream">The stream to deserialize the data from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        T Deserialize(Stream stream);
    }
}