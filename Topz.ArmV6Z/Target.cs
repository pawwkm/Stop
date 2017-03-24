using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A reference to some <see cref="Label"/> or atom.
    /// </summary>
    internal sealed class Target : Operand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Target"/> class.
        /// </summary>
        /// <param name="name">The name of the target.</param>
        /// <param name="position">The position of the target in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Target(string name, InputPosition position) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        /// <summary>
        /// The name of the target.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}