using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Performs a bitwise and operation on two values.
    /// </summary>
    /// <remarks>See A.4.1.4 for more info.</remarks>
    internal sealed class AndInstruction : Format1Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndInstruction"/> class.
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
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.And"/>.
        /// </exception>
        public AndInstruction(Mnemonic mnemonic, Register rd, Register rm, AddressingMode1 shifter) : base(mnemonic, rd, rm, shifter)
        {
            if (mnemonic.RawName != Mnemonic.And)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.And}", nameof(mnemonic));
        }
    }
}