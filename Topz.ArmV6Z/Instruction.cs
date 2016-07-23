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
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> is null.
        /// </exception>
        protected Instruction(Label label, Mnemonic mnemonic) : base(mnemonic.Position)
        {
            if (mnemonic == null)
                throw new ArgumentNullException(nameof(mnemonic));

            Label = label;
            Mnemonic = mnemonic;
        }

        /// <summary>
        /// The label of the instruction. 
        /// If null, this instruction doesn't have any label.
        /// </summary>
        public Label Label
        {
            get;
            set;
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