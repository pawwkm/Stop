using System;
using System.Text;

namespace Stop.FileFormats
{
    /// <summary>
    /// Some null terminated textual data.
    /// </summary>
    public sealed class NullTerminatedString : Atom
    {
        private string content = "";

        /// <summary>
        /// The actual text of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                content = value;
            }
        }

        /// <summary>
        /// The size of the atom, in bytes.
        /// </summary>
        public override uint Size
        {
            get
            {
                return (uint)Encoding.UTF8.GetByteCount(Content);
            }
        }
    }
}