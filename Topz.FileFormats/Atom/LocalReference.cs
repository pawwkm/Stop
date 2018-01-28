namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Reference to a local address of a <see cref="Procedure"/>.
    /// </summary>
    public sealed class LocalReference : Reference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalReference"/> class.
        /// </summary>
        /// <param name="target">This is the address within the procedure that is referred to.</param>
        public LocalReference(uint target)
        {
            Target = target;
        }

        /// <summary>
        /// This is the address within the procedure that is referred to.
        /// </summary>
        public uint Target
        {
            get;
            private set;
        }
    }
}