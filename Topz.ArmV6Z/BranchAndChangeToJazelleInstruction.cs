using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Branches to an address and enters the Jazelle state if Jazelle is 
    /// available. Otherwise this behaves as <see cref="BranchAndExchangeInstruction"/>.
    /// </summary>
    internal sealed class BranchAndChangeToJazelleInstruction : Format4Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAndChangeToJazelleInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        public BranchAndChangeToJazelleInstruction(Label label, Mnemonic mnemonic, RegisterOperand operand) : base(label, mnemonic, operand)
        {
        }
    }
}