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
        /// <param name="register">The register whose value is to be shifted.</param>
        /// <param name="shift">The value that the <paramref name="register"/> is being left shifted by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="shift"/> is not between 0 - 31.
        /// </exception>
        public LogicalLeftShiftByImmediateOperand(Register register, int shift)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (shift < 0 || shift > 31)
                throw new ArgumentOutOfRangeException(nameof(shift));

            Register = register;
            Shift = shift;
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
        /// The value that the <see cref="Register"/> is being left shifted by.
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
            return $"{Register}{Symbols.Comma} {Register.Lsl} #{Shift}";
        }
    }
}