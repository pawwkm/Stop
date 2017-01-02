using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Data-processing operands - Immediate.
    /// </summary>
    /// <remarks>See section A5.1.3</remarks>
    internal sealed class ImmediateOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmediateOperand"/> class.
        /// </summary>
        /// <param name="value">The immediate value.</param>
        /// <param name="position">The position where the immediate operand was declared.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> doesn't fit within 12 bits.
        /// </exception>
        public ImmediateOperand(ushort value, InputPosition position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (value > 4095)
                throw new ArgumentOutOfRangeException(nameof(value), "The value doesn't fit within 12 bits.");

            Value = value;
            DefinedAt = position;
        }

        /// <summary>
        /// The immediate value.
        /// </summary>
        public ushort Value
        {
            get;
            private set;
        }

        /// <summary>
        /// The position where the immediate operand was declared.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }
    }
}