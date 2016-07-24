using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines a 16 bit immediate integer operand.
    /// </summary>
    internal class Immediate16Operand
    {
        /// <summary>
        /// Initailizes a new instance of the <see cref="Immediate16Operand"/> class.
        /// </summary>
        /// <param name="position">The position where the operand was declared.</param>
        /// <param name="value">The value of the operand.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Immediate16Operand(InputPosition position, ushort value)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Position = position;
            Value = value;
        }

        /// <summary>
        /// The position where the operand was declared.
        /// </summary>
        public InputPosition Position
        {
            get;
            private set;
        }

        /// <summary>
        /// The value of the operand.
        /// </summary>
        public ushort Value
        {
            get;
            private set;
        }
    }
}