using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Compares one value with the twos complement of a second value. The first value 
    /// comes from a register. The second value can be either an immediate value or a value 
    /// from a register, and can be shifted before the comparison.
    /// </summary>
    internal sealed class CompareNegativeInstruction : Format6Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNegativeInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The shifter operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        public CompareNegativeInstruction(Label label, Mnemonic mnemonic, RegisterOperand first, ShifterOperand second) : base(label, mnemonic, first, second)
        {
        }
    }
}