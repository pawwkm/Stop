using System;

namespace Topz.ArmV6Z
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
        /// <param name="first">The first register operand of the instruction.</param>
        /// <param name="second">The second register operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Clz"/>.
        /// </exception>
        public CountLeadingZeroesInstruction(Mnemonic mnemonic, Register first, Register second) : base(mnemonic, first, second)
        {
            if (mnemonic.RawName != Mnemonic.Clz)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Clz}", nameof(mnemonic));
        }
    }
}