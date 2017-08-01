namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Reference to a local address of a <see cref="Procedure"/>.
    /// </summary>
    public sealed class LocalReference : Reference
    {
        /// <summary>
        /// This is the address within the procedure that is refered to.
        /// </summary>
        public uint Target
        {
            get;
            set;
        }
    }
}