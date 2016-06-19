namespace Topz.FileSystems
{
    /// <summary>
    /// The status of a <see cref="Partition"/>.
    /// </summary>
    public enum PartitionStatus
    {
        /// <summary>
        /// The partition is not bootable.
        /// </summary>
        Inactive,

        /// <summary>
        /// The partition is bootable.
        /// </summary>
        Bootable,

        /// <summary>
        /// The partition is invalid.
        /// </summary>
        Invalid
    }
}