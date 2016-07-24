using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Returns the number of binary zero bits before the first binary one bit in a value. 
    /// </summary>
    internal sealed class CopyInstruction : Format5Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The first register operand of the instruction.</param>
        /// <param name="second">The second register operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        public CopyInstruction(Label label, Mnemonic mnemonic, RegisterOperand first, RegisterOperand second) : base(label, mnemonic, first, second)
        {
        }
    }
}