using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Load register instruction.
    /// </summary>
    /// <remarks>See section A4.1.23</remarks>
    internal sealed class LoadRegisterInstruction : Format7Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadRegisterInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The addressing mode operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        public LoadRegisterInstruction(Label label, Mnemonic mnemonic, RegisterOperand first, AddressingModeOperand second) : base(label, mnemonic, first, second)
        {
        }
    }
}