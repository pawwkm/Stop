using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Data-processing operands - Rotate right by register.
    /// </summary>
    /// <remarks>See section A5.1.12</remarks>
    internal sealed class RotateRightByRegisterOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateRightByRegisterOperand"/> class.
        /// </summary>
        /// <param name="register">The register whose value is to be shifted.</param>
        /// <param name="rotation">The register that the <paramref name="register"/> is being right rotated by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> or <paramref name="rotation"/> is null.
        /// </exception>
        public RotateRightByRegisterOperand(Register register, Register rotation)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));

            Register = register;
            Rotation = rotation;
        }

        /// <summary>
        /// The register whose value is to be rotated.
        /// </summary>
        public Register Register
        {
            get;
            private set;
        }

        /// <summary>
        /// The value that the <see cref="Register"/> is being right rotated by.
        /// </summary>
        public Register Rotation
        {
            get;
            private set;
        }
    }
}