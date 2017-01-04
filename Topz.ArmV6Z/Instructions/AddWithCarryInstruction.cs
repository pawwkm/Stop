using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// The add with carry instruction.
    /// </summary>
    /// <remarks>See A.4.1.2 for more info.</remarks>
    internal sealed class AddWithCarryInstruction : Format1Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddWithCarryInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rd">The first register.</param>
        /// <param name="rm">The second register.</param>
        /// <param name="shifter">The addressing mode.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="rd"/>,
        /// <paramref name="rm"/> or <paramref name="shifter"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Adc"/>.
        /// </exception>
        public AddWithCarryInstruction(Mnemonic mnemonic, Register rd, Register rm, AddressingMode1 shifter) : base(mnemonic, rd, rm, shifter)
        {
            if (mnemonic.RawName != Mnemonic.Adc)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Adc}", nameof(mnemonic));
        }
    }
}