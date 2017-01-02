using System;
using Pote;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Load and Store Word or Unsigned Byte - Immediate offset.
    /// </summary>
    /// <remarks>See section A5.2.2</remarks>
    internal sealed class ImmediateOffsetOperand : AddressingMode2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmediateOffsetOperand"/> class.
        /// </summary>
        /// <param name="register">Specifies the register containing the base address.</param>
        /// <param name="offset">
        /// Specifies the immediate offset used with the value of 
        /// <paramref name="register"/> to form the address.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="offset"/> doesn't fit within 12 bits.
        /// </exception>
        public ImmediateOffsetOperand(RegisterOperand register, int offset) : base(register)
        {
            if (offset > 2047 || offset < -2048)
                throw new ArgumentOutOfRangeException(nameof(offset), "This doesn't fit within 12 bits.");

            Offset = offset;
        }

        /// <summary>
        /// Specifies the immediate offset used with the value of 
        /// <see cref="AddressingMode2.BaseAddress"/> to form the address.
        /// </summary>
        public int Offset
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string that represents the current offset.
        /// </summary>
        /// <returns>A string that represents the current offset.</returns>
        public override string ToString()
        {
            return $"#{Offset}";
        }
    }
}