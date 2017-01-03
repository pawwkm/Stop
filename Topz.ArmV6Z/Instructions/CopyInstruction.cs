using System;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Returns the number of binary zero bits before the first binary one bit in a value. 
    /// </summary>
    /// <remarks>See section A4.1.17</remarks>
    internal sealed class CopyInstruction : Format5Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The first register operand of the instruction.</param>
        /// <param name="second">The second register operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Cpy"/>.
        /// </exception>
        public CopyInstruction(Mnemonic mnemonic, Register first, Register second) : base(mnemonic, first, second)
        {
            if (mnemonic.RawName != Mnemonic.Cpy)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Cpy}", nameof(mnemonic));
        }
    }
}