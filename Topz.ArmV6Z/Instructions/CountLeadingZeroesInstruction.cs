using System;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Returns the number of binary zero bits before the first binary one bit in a value. 
    /// </summary>
    /// <remarks>See section A4.1.13</remarks>
    internal sealed class CountLeadingZeroesInstruction : Format5Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountLeadingZeroesInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rd">The first register operand of the instruction.</param>
        /// <param name="rm">The second register operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="rd"/> or <paramref name="rm"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Clz"/>.
        /// </exception>
        public CountLeadingZeroesInstruction(Mnemonic mnemonic, Register rd, Register rm) : base(mnemonic, rd, rm)
        {
            if (mnemonic.RawName != Mnemonic.Clz)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Clz}", nameof(mnemonic));
        }
    }
}