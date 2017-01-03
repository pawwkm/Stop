using System;

namespace Topz.ArmV6Z.Operands
{
    /// <summary>
    /// Data-processing operands - Rotate right by immediate.
    /// </summary>
    /// <remarks>See section A5.1.11</remarks>
    internal sealed class RotateRightByImmediateOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateRightByImmediateOperand"/> class.
        /// </summary>
        /// <param name="register">The register whose value is to be rotated.</param>
        /// <param name="rotation">The value that the <paramref name="register"/> is being right rotated by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="rotation"/> is not between 1 - 31.
        /// </exception>
        public RotateRightByImmediateOperand(Register register, int rotation)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (rotation < 1 || rotation > 31)
                throw new ArgumentOutOfRangeException(nameof(rotation));

            Register = register;
            Rotation = rotation;
        }

        /// <summary>
        /// The register whose value is to be shifted.
        /// </summary>
        public Register Register
        {
            get;
            private set;
        }

        /// <summary>
        /// The value that the <see cref="Register"/> is being right rotated by.
        /// </summary>
        public int Rotation
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string that represents the current operand.
        /// </summary>
        /// <returns>A string that represents the current operand.</returns>
        public override string ToString()
        {
            return $"{Register}{Symbols.Comma} {Register.Ror} #{Rotation}";
        }
    }
}