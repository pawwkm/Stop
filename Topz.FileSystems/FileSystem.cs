using System;
using System.IO;
using System.Reflection;

namespace Topz.FileSystems
{
    /// <summary>
    /// Defines a file system.
    /// </summary>
    public abstract class FileSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystem"/> class.
        /// </summary>
        /// <param name="stream">The stream containing the image of the file system.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="stream"/> cannot be read or written or seeked.
        /// </exception>
        protected FileSystem(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            else if (!stream.CanRead)
                throw new ArgumentException("The stream cannot be read from.", "stream");
            else if (!stream.CanWrite)
                throw new ArgumentException("The stream cannot be written to.", "stream");
            else if (!stream.CanSeek)
                throw new ArgumentException("The stream is not seekable.", "stream");

            Source = stream;
        }

        /// <summary>
        /// The stream containing the image of the file system.
        /// </summary>
        public Stream Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Moves the specified file or directory.
        /// </summary>
        /// <param name="oldPath">The old path of the file or directory.</param>
        /// <param name="newPath">
        /// The new path of the file or directory.
        /// If there is a file or directory at this path, the 
        /// operation will fail.
        /// </param>
        /// <returns>True if the file or directory was successfully moved.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldPath"/> or <paramref name="newPath"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Move(string oldPath, string newPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Moves the specified file or directory.
        /// </summary>
        /// <param name="oldPath">The old path of the file or directory.</param>
        /// <param name="newPath">
        /// The new path of the file or directory.
        /// If there is a file or directory at this path and <paramref name="force"/>
        /// is false, the operation will fail.
        /// </param>
        /// <param name="force">
        /// If true, then if it is a file it is overwritten. If it is a directory then the
        /// contents from the old one is moved to the new one.
        /// </param>
        /// <returns>True if the file or directory was successfully moved.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldPath"/> or <paramref name="newPath"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Move(string oldPath, string newPath, bool force)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Rename the specified file or directory.
        /// </summary>
        /// <param name="oldName">The old name of the file or directory.</param>
        /// <param name="newName">
        /// The new name of the file or directory.
        /// </param>
        /// <returns>True if the file or directory was renamed successfully.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldName"/> or <paramref name="newName"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Rename(string oldName, string newName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Rename the specified file or directory.
        /// </summary>
        /// <param name="oldName">The old name of the file or directory.</param>
        /// <param name="newName">The new name of the file or directory.</param>
        /// <param name="force">
        /// If true, then if it is a file it is overwritten. If it is a directory then the
        /// contents from the old one is moved to the new one.
        /// </param>
        /// <returns>True if the file or directory was renamed successfully.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldName"/> or <paramref name="newName"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Rename(string oldName, string newName, bool force)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks if a directory or file exists in the file system.
        /// </summary>
        /// <param name="path">The path of the directory or file.</param>
        /// <returns>true if the directory or file exists; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Exist(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a file or a directory in the file system.
        /// </summary>
        /// <param name="path">The path to the new file or directory.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual void Create(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Opens the file specified by the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the file to open.</param>
        /// <returns>The opened file stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// If the given <paramref name="path"/> contains a directory that doesn't exist
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// If the given <paramref name="path"/> is to a file and it doesn't exist
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual Stream Open(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Deletes the file or directory specified by the given <paramref name="path"/>
        /// from the file system.
        /// </summary>
        /// <param name="path">The path of the file or directory to delete.</param>
        /// <returns>True if the file or directory was deleted; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Delete(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Undeletes a file or a directory.
        /// </summary>
        /// <param name="path">
        /// The path to file or directory to undelete.
        /// If there already is a file or directory at 
        /// the same path, the operation fails.
        /// </param>
        /// <returns>True if the file or directory could be undeleted.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Undelete(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Un-deletes a file or a directory.
        /// </summary>
        /// <param name="path">
        /// The path to file or directory to un-delete.
        /// If there already is a file or directory at 
        /// the same path, the operation must be forced 
        /// for it to succeed.
        /// </param>
        /// <param name="force">
        /// If set to <c>true</c>, the un-deletion is forced then it 
        /// overwrites the file or directory at the given <paramref name="path"/>.
        /// </param>
        /// <returns>True if the un-deletion was successful.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If not overridden in a sub class.
        /// </exception>
        public virtual bool Undelete(string path, bool force)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the <paramref name="attribute"/> of the <paramref name="target"/>.
        /// </summary>
        /// <param name="target">Target entity of the file system.</param>
        /// <param name="attribute">The name of the attribute that is to be set.</param>
        /// <param name="value">The new value of the <paramref name="attribute"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/>, <paramref name="attribute"/> or <paramref name="value"/>
        /// is null. 
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The file system doesn't support setting the attribute.
        /// If not overridden in a sub class.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Unknown <paramref name="target"/> or <paramref name="attribute"/>. 
        /// Invalid <paramref name="value"/>.
        /// </exception>
        public virtual void SetAttribute(string target, string attribute, string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads the specified structure <typeparamref name="T"/> 
        /// at the specified <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The type of structure to read from the file system.</typeparam>
        /// <param name="position">The starting position in the <see cref="Source"/>.</param>
        /// <returns>The structure read from the file system.</returns>
        /// <exception cref="ArgumentException">
        /// <typeparamref name="T"/> cannot be marshaled as an unmanaged structure;
        /// no meaningful size or offset can be computed.
        /// </exception>
        protected T ReadStructure<T>(long position)
        {
            Source.Position = position;

            var attribute = typeof(T).GetCustomAttribute<SerializerAttribute>(true);
            ISerializer<T> serializer = (ISerializer<T>)Activator.CreateInstance(attribute.Serializer);

            return serializer.Deserialize(Source);
        }

        /// <summary>
        /// Writes a structure to the image.
        /// </summary>
        /// <typeparam name="T">The type of structure to write to the image.</typeparam>
        /// <param name="position">The position to start writing.</param>
        /// <param name="structure">The structure to write.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="structure"/> is null.
        /// </exception>
        protected void WriteStructure<T>(long position, T structure)
        {
            Source.Position = position;

            var attribute = typeof(T).GetCustomAttribute<SerializerAttribute>(true);
            ISerializer<T> serializer = (ISerializer<T>)Activator.CreateInstance(attribute.Serializer);

            serializer.Serialize(structure, Source);
        }
    }
}