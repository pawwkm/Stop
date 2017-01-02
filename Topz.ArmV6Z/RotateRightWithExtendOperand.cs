using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Data-processing operands - Rotate right with extend.
    /// </summary>
    /// <remarks>See section A5.1.13</remarks>
    internal sealed class RotateRightWithExtendOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateRightWithExtendOperand"/> class.
        /// </summary>
        /// <param name="register">The register to right shift by one bit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> is null.
        /// </exception>
        public RotateRightWithExtendOperand(Register register)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));

            Register = register;
        }

        /// <summary>
        /// The register to right shift by one bit.
        /// </summary>
        public Register Register
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
            return $"{Register}{Symbols.Comma} {Register.Rrx}";
        }
    }
}