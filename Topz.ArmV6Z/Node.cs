using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A node in an abstract syntax tree.
    /// </summary>
    internal abstract class Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        protected Node(InputPosition position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Position = position;
        }

        /// <summary>
        /// The position of the node in the program's source code.
        /// </summary>
        public InputPosition Position
        {
            get;
            private set;
        }

        /// <summary>
        /// Accepts a given <paramref name="pass"/>.
        /// </summary>
        /// <param name="pass">The pass to accept.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pass"/> is null.
        /// </exception>
        public void Accept(IPass pass)
        {
            if (pass == null)
                throw new ArgumentNullException(nameof(pass));

            pass.Visit((dynamic)this);
        }
    }
}