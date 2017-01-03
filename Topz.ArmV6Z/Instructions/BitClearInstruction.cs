using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Performs a bitwise AND of one value with the complement of a second value. 
    /// </summary>
    /// <remarks>See A.4.1.6 for more info.</remarks>
    internal sealed class BitClearInstruction : Format1Instruction
    {
        /// <summary>
        /// Intializes a new instance of the <see cref="BitClearInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="r1">The first register.</param>
        /// <param name="r2">The second register.</param>
        /// <param name="shifter">The addressing mode.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="r1"/>,
        /// <paramref name="r2"/> or <paramref name="shifter"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Bic"/>.
        /// </exception>
        public BitClearInstruction(Mnemonic mnemonic, Register r1, Register r2, AddressingMode1 shifter) : base(mnemonic, r1, r2, shifter)
        {
            if (mnemonic.RawName != Mnemonic.Bic)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Bic}", nameof(mnemonic));
        }
    }
}