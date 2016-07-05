using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a string in a program.
    /// </summary>
    internal sealed class String : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="String"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="content">The content of the string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="content"/> is null.
        /// </exception>
        public String(InputPosition position, string content) : base(position)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            Content = content;
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