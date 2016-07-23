using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A branch instruction.
    /// </summary>
    /// <remarks>See A.4.1.5 for more info.</remarks>
    internal class BranchInstruction : Format2Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The target operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        public BranchInstruction(Label label, Mnemonic mnemonic, TargetOperand operand) : base(label, mnemonic, operand)
        {
        }
    }
}