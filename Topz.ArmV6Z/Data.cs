using Pote.Text;
using System;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a piece of data in a program.
    /// </summary>
    internal sealed class Data : Node, INamedNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="name">Name of the data.</param>
        /// <param name="content">The actual data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/>, <paramref name="name"/> or <paramref name="content"/> is null.
        /// </exception>
        public Data(InputPosition position, string name, IEnumerable<byte> content) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            Name = name;
            Content = new List<byte>(content);
        }

        /// <summary>
        /// Name of the data.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The actual data.
        /// </summary>
        public IList<byte> Content
        {
            get;
            private set;
        }
    }
}