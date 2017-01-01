using System;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Addressing Mode 2 - Load and Store Word or Unsigned Byte.
    /// </summary>
    /// <remarks>See section A5.2</remarks>
    internal abstract class AddressingModeOperand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressingModeOperand"/> class.
        /// </summary>
        /// <param name="register">Specifies the register containing the base address.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="register"/> is not one of <see cref="Registers.All"/>.
        /// </exception>
        protected AddressingModeOperand(string register)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (!Registers.All.Contains(register))
                throw new ArgumentException($"'{register}' is not a register.", nameof(register));

            BaseRegister = register;
        }

        /// <summary>
        /// Specifies the register containing the base address.
        /// </summary>
        public string BaseRegister
        {
            get;
            private set;
        }
    }
}