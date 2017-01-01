using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic, register, addressing mode</para>
    /// </summary>
    /// <remarks>
    /// See section A5.2
    /// </remarks>
    internal abstract class Format7Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format7Instruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The addressing mode operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        protected Format7Instruction(Label label, Mnemonic mnemonic, RegisterOperand first, AddressingModeOperand second) : base(label, mnemonic)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            First = first;
            Second = second;
        }

        /// <summary>
        /// The register operand of the instruction.
        /// </summary>
        public RegisterOperand First
        {
            get;
            private set;
        }

        /// <summary>
        /// The addressing mode operand of the instruction.
        /// </summary>
        public AddressingModeOperand Second
        {
            get;
            private set;
        }
    }
}