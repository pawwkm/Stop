using System;

namespace Topz.ArmV6Z.Operands
{
    /// <summary>
    /// Data-processing operands - Logical shift right by register.
    /// </summary>
    /// <remarks>See section A5.1.8</remarks>
    internal sealed class LogicalRightShiftByRegisterOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalRightShiftByRegisterOperand"/> class.
        /// </summary>
        /// <param name="rm">The register whose value is to be shifted.</param>
        /// <param name="rs">The register that the <paramref name="rm"/> is being right shifted by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rm"/> or <paramref name="rs"/> is null.
        /// </exception>
        public LogicalRightShiftByRegisterOperand(Register rm, Register rs)
        {
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));

            Rm = rm;
            Rs = rs;
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
        /// The value that the <see cref="Rm"/> is being right shifted by.
        /// </summary>
        public Register Rs
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
            return $"{Rm}{Symbols.Comma} {Register.Lsr} {Rs}";
        }
    }
}