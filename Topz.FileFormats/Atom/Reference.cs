namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Defines a reference from a <see cref="Procedure"/> to an <see cref="Atom"/>.
    /// </summary>
    public abstract class Reference
    {
        /// <summary>
        /// The type of address to relocate.
        /// </summary>
        public AddressType AddressType
        {
            get;
            set;
        }

        /// <summary>
        /// This is the address within the code block of the procedure to
        /// relocate to match the address of the referenced atom.
        /// </summary>
        public uint Address
        {
            get;
            set;
        }
    }
}