using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a label in a program.
    /// </summary>
    internal sealed class Label : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="name">The actual label.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> doesn't start with a letter, digit or '_'.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Label(InputPosition position, string name) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length == 0 || !char.IsLetterOrDigit(name[0]) && name[0] != '_')
                throw new ArgumentException("The label doesn't start with a letter, digit or '_'.", nameof(name));

            Name = name;
        }

        /// <summary>
        /// The actual label.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The address of the label counting from the beginning of
        /// the containing <see cref="Procedure"/>.
        /// </summary>
        public uint Address
        {
            get;
            set;
        }
    }
}