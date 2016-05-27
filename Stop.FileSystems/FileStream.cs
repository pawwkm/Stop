using System;
using System.IO;

namespace Stop.FileSystems
{
    /// <summary>
    /// A file stream from a virtual file system.
    /// </summary>
    internal class FileStream : MemoryStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStream"/> class.
        /// </summary>
        public FileStream()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStream"/> class.
        /// </summary>
        /// <param name="buffer">The content of the file.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        public FileStream(byte[] buffer) : base(buffer)
        {
        }

        /// <summary>
        /// Occurs when the file has been flushed.
        /// </summary>
        public event EventHandler AfterFlush;

        /// <summary>
        /// Occurs when the file has been closed.
        /// </summary>
        public event EventHandler AfterClose;

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered 
        /// data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            base.Flush();
            OnAfterFlush(EventArgs.Empty);
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and
        /// file handles) associated with the current stream. Instead of calling this
        /// method, ensure that the stream is properly disposed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            OnAfterClose(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="AfterFlush"/> event.
        /// </summary>
        /// <param name="args">
        /// An <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnAfterFlush(EventArgs args)
        {
            if (AfterFlush != null)
                AfterFlush(this, args);
        }

        /// <summary>
        /// Raises the <see cref="AfterClose"/> event.
        /// </summary>
        /// <param name="args">
        /// An <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnAfterClose(EventArgs args)
        {
            if (AfterClose != null)
                AfterClose(this, args);
        }
    }
}