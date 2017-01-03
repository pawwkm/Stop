﻿using System;

namespace Topz.ArmV6Z.Operands
{
    /// <summary>
    /// Addressing Mode 2 - Load and Store Word or Unsigned Byte.
    /// </summary>
    /// <remarks>See section A5.2</remarks>
    internal abstract class AddressingMode2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressingMode2"/> class.
        /// </summary>
        /// <param name="baseAddress">Specifies the register containing the base address.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseAddress"/> is null.
        /// </exception>
        protected AddressingMode2(RegisterOperand baseAddress)
        {
            if (baseAddress == null)
                throw new ArgumentNullException(nameof(baseAddress));

            BaseAddress = baseAddress;
        }

        /// <summary>
        /// Specifies the register containing the base address.
        /// </summary>
        public RegisterOperand BaseAddress
        {
            get;
            private set;
        }
    }
}