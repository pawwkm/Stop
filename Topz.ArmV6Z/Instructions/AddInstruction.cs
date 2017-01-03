using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Adds two values together.
    /// </summary>
    /// <remarks>See A.4.1.3 for more info.</remarks>
    internal sealed class AddInstruction : Format1Instruction
    {
        /// <summary>
        /// Intializes a new instance of the <see cref="AddInstruction"/> class.
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
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Add"/>.
        /// </exception>
        public AddInstruction(Mnemonic mnemonic, Register rd, Register rm, AddressingMode1 shifter) : base(mnemonic, rd, rm, shifter)
        {
            if (mnemonic.RawName != Mnemonic.Add)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Add}", nameof(mnemonic));
        }
    }
}