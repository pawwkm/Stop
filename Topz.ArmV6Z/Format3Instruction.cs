using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic, immediate 16</para>
    /// </summary>
    internal abstract class Format3Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format3Instruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        protected Format3Instruction(Label label, Mnemonic mnemonic, Immediate16Operand operand) : base(label, mnemonic)
        {
            if (operand == null)
                throw new ArgumentNullException(nameof(operand));

            Operand = operand;
        }

        /// <summary>
        /// The operand of the instruction.
        /// </summary>
        public Immediate16Operand Operand
        {
            get;
            private set;
        }
    }
}