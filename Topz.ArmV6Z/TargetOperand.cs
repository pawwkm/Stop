using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A target address operand.
    /// </summary>
    internal sealed class TargetOperand : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetOperand"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="target">The target address.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="target"/> is beyond a 24 bit signed integer.
        /// </exception>
        public TargetOperand(InputPosition position, int target) : base(position)
        {
            if (target > 8388607 || target < -8388608)
                throw new ArgumentOutOfRangeException(nameof(target));

            Target = target;
        }

        /// <summary>
        /// The target address.
        /// </summary>
        public int Target
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
            return $"#{Target}";
        }
    }
}