using Topz.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// The base of all operand types.
    /// </summary>
    internal abstract class Operand : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operand"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        protected Operand(Position position) : base(position)
        {
        }
    }
}