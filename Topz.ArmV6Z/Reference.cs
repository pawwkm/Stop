using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A reference to a label or an atom.
    /// </summary>
    internal sealed class Reference : Operand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        /// <param name="name">The name of the label or atom being referenced.</param>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> or <paramref name="position"/> is null.
        /// </exception>
        public Reference(string name, InputPosition position) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        /// <summary>
        /// The name of the label or atom being referenced.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}