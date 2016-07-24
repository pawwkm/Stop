using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic, register, shifter operand</para>
    /// </summary>
    internal abstract class Format6Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format6Instruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The shifter operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        protected Format6Instruction(Label label, Mnemonic mnemonic, RegisterOperand first, ShifterOperand second) : base(label, mnemonic)
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
        /// The shifter operand of the instruction.
        /// </summary>
        public ShifterOperand Second
        {
            get;
            private set;
        }
    }
}