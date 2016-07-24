using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A break point instruction.
    /// </summary>
    internal class BreakPointInstruction : Format3Instruction
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
        public BreakPointInstruction(Label label, Mnemonic mnemonic, Immediate16Operand operand) : base(label, mnemonic, operand)
        {
        }
    }
}