using System;

namespace Topz.ArmV6Z.Operands
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
        /// <param name="rm">The register to right shift by one bit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rm"/> is null.
        /// </exception>
        public RotateRightWithExtendOperand(Register rm)
        {
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));

            Rm = rm;
        }

        /// <summary>
        /// The register to right shift by one bit.
        /// </summary>
        public Register Rm
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
            return $"{Rm}{Symbols.Comma} {Register.Rrx}";
        }
    }
}