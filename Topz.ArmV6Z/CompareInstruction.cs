using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Compares two values.
    /// </summary>
    internal class CompareInstruction : Format6Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The shifter operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        public CompareInstruction(Label label, Mnemonic mnemonic, RegisterOperand first, ShifterOperand second) : base(label, mnemonic, first, second)
        {
        }
    }
}