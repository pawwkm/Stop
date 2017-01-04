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
        /// <param name="rm">The register whose value is to be rotated.</param>
        /// <param name="rotation">The value that the <paramref name="rm"/> is being right rotated by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rm"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="rotation"/> is not between 1 - 31.
        /// </exception>
        public RotateRightByImmediateOperand(Register rm, int rotation)
        {
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));
            if (rotation < 1 || rotation > 31)
                throw new ArgumentOutOfRangeException(nameof(rotation));

            Rm = rm;
            Rotation = rotation;
        }

        /// <summary>
        /// The register whose value is to be shifted.
        /// </summary>
        public Register Rm
        {
            get;
            private set;
        }

        /// <summary>
        /// The value that the <see cref="Rm"/> is being right rotated by.
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
            return $"{Rm}{Symbols.Comma} {Register.Ror} #{Rotation}";
        }
    }
}