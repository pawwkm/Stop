﻿using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Data-processing operands - Arithmetic shift right by register.
    /// </summary>
    /// <remarks>See section A5.1.10</remarks>
    internal sealed class ArithmeticRightShiftByRegisterOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticRightShiftByRegisterOperand"/> class.
        /// </summary>
        /// <param name="register">The register whose value is to be shifted.</param>
        /// <param name="shift">The register that the <paramref name="register"/> is being right shifted by.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> or <paramref name="shift"/> is null.
        /// </exception>
        public ArithmeticRightShiftByRegisterOperand(Register register, Register shift)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));

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
        public Register Shift
        {
            get;
            private set;
        }
    }
}