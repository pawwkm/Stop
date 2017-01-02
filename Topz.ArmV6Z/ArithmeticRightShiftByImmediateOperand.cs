using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Data-processing operands - Arithmetic shift right by immediate.
    /// </summary>
    /// <remarks>See section A5.1.9</remarks>
    internal sealed class ArithmeticRightShiftByImmediateOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticRightShiftByImmediateOperand"/> class.
        /// </summary>
        /// <param name="register">The register whose value is to be shifted.</param>
        /// <param name="shift">The value that the <paramref name="register"/> is being right shifted by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="shift"/> is not between 1 - 32.
        /// </exception>
        public ArithmeticRightShiftByImmediateOperand(Register register, int shift)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (shift < 1 || shift > 32)
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
        /// The value that the <see cref="Register"/> is being right shifted by.
        /// </summary>
        public int Shift
        {
            get;
            private set;
        }
    }
}