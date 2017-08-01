namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Defines the address types available.
    /// </summary>
    public enum AddressType : byte
    {
        /// <summary>
        /// The address is encoded as defined in the Arm 
        /// reference manual section A4.1.5.
        /// </summary>
        ArmTargetAddress = 0
    }
}