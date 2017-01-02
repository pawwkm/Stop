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
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The addressing mode operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Adc"/>.
        /// </exception>
        public LoadRegisterInstruction(Mnemonic mnemonic, RegisterOperand first, AddressingMode2 second) : base(mnemonic, first, second)
        {
            if (mnemonic.RawName != Mnemonic.Ldr)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Ldr}", nameof(mnemonic));
        }
    }
}