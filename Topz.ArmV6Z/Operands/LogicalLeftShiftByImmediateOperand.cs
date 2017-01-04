using System;

namespace Topz.ArmV6Z.Operands
{
    /// <summary>
    /// Data-processing operands - Logical shift left by immediate.
    /// </summary>
    /// <remarks>See section A5.1.5</remarks>
    internal sealed class LogicalLeftShiftByImmediateOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalLeftShiftByImmediateOperand"/> class.
        /// </summary>
        /// <param name="rm">The register whose value is to be shifted.</param>
        /// <param name="shift">The value that the <paramref name="rm"/> is being left shifted by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rm"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="shift"/> is not between 0 - 31.
        /// </exception>
        public LogicalLeftShiftByImmediateOperand(Register rm, int shift)
        {
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));
            if (shift < 0 || shift > 31)
                throw new ArgumentOutOfRangeException(nameof(shift));

            Rm = rm;
            Shift = shift;
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
        /// The value that the <see cref="Rm"/> is being left shifted by.
        /// </summary>
        public int Shift
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
            return $"{Rm}{Symbols.Comma} {Register.Lsl} #{Shift}";
        }
    }
}