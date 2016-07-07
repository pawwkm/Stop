using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents an instruction in a program.
    /// </summary>
    internal abstract class Instruction : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="position">The position of the instruction in the program's source code.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="mnemonic"/> is null.
        /// </exception>
        protected Instruction(InputPosition position, Mnemonic mnemonic) : base(position)
        {
        }

        /// <summary>
        /// The label of the instruction. 
        /// If null, this instruction doesn't have any label.
        /// </summary>
        public Label Label
        {
            get;
            private set;
        }

        /// <summary>
        /// The mnemonic of the instruction.
        /// </summary>
        public Mnemonic Mnemonic
        {
            get;
            private set;
        }
    }
}