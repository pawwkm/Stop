using System.Collections.Generic;

namespace Stop.FileFormats
{
    /// <summary>
    /// Data blocks can be any chunk of data that doesn't fit in as 
    /// one of the other <see cref="Atom"/> types.
    /// </summary>
    public sealed class Data : Atom
    {
        private List<byte> content = new List<byte>();

        /// <summary>
        /// The content of the data block.
        /// </summary>
        public IList<byte> Content
        {
            get
            {
                return content;
            }
        }

        /// <summary>
        /// The size of the atom, in bytes.
        /// </summary>
        public override uint Size
        {
            get
            {
                return (uint)Content.Count;
            }
        }
    }
}