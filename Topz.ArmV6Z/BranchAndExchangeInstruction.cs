using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Branches to an address, with an optional switch to Thumb state.
    /// </summary>
    internal sealed class BranchAndExchangeInstruction : Format4Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAndExchangeInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        public BranchAndExchangeInstruction(Label label, Mnemonic mnemonic, RegisterOperand operand) : base(label, mnemonic, operand)
        {
        }
    }
}