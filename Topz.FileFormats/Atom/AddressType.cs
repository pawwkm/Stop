using System.Diagnostics.CodeAnalysis;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Defines the address types available.
    /// </summary>

    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = Justifications.UnderlyingTypeIsRequired)]
    public enum AddressType : byte
    {
        /// <summary>
        /// The address is encoded as defined in the Arm 
        /// Architecture Reference Manual section A4.1.5.
        /// </summary>
        ArmTargetAddress = 0,

        /// <summary>
        /// The address is encoded as defined in the Arm 
        /// Architecture Reference Manual section A5.2.
        /// </summary>
        ArmOffset12 = 1
    }
}