using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a string in a program.
    /// </summary>
    internal sealed class String : Node, INamedNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="String"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="name">The name of the string.</param>
        /// <param name="content">The content of the string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/>,  or <paramref name="content"/> is null.
        /// </exception>
        public String(InputPosition position, string name, string content) : base(position)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            Name = name;
            Content = content;
        }

        /// <summary>
        /// Name of the string.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The content of the string.
        /// </summary>
        public string Content
        {
            get;
            private set;
        }
    }
}