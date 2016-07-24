using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic, register, register</para>
    /// </summary>
    internal abstract class Format5Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format4Instruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The first register operand of the instruction.</param>
        /// <param name="second">The second register operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        protected Format5Instruction(Label label, Mnemonic mnemonic, RegisterOperand first, RegisterOperand second) : base(label, mnemonic)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            First = first;
            Second = second;
        }

        /// <summary>
        /// The first register operand of the instruction.
        /// </summary>
        public RegisterOperand First
        {
            get;
            private set;
        }

        /// <summary>
        /// The second register operand of the instruction.
        /// </summary>
        public RegisterOperand Second
        {
            get;
            private set;
        }
    }
}