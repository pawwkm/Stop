using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A single procedure in a program.
    /// </summary>
    internal sealed class Procedure : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Procedure"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Procedure(InputPosition position) : base(position)
        {
        }
    }
}