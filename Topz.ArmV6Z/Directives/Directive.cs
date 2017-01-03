﻿using Pote.Text;
using System;

namespace Topz.ArmV6Z.Directives
{
    /// <summary>
    /// An assembler directive.
    /// </summary>
    internal abstract class Directive : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Directive"/> class.
        /// </summary>
        /// <param name="position">The position of the directive in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        protected Directive(InputPosition position) : base(position)
        {
        }
    }
}