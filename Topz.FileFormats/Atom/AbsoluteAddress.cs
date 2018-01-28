namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// An absolute address that cen be referenced.
    /// </summary>
    public sealed class AbsoluteAddress : Atom
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbsoluteAddress"/> class.
        /// </summary>
        /// <param name="address">The actual address.</param>
        public AbsoluteAddress(ulong address)
        {
            Address = address;
        }

        /// <summary>
        /// The size of the atom, in bytes.
        /// </summary>
        public override uint Size
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// The actual address.
        /// </summary>
        public ulong Address
        {
            get;
            private set;
        }
    }
}