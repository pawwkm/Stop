using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <remarks>See section A4.1.15</remarks>
    internal sealed class CompareInstruction : Format6Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The shifter operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Cmp"/>.
        /// </exception>
        public CompareInstruction(Mnemonic mnemonic, Register first, AddressingMode1 second) : base(mnemonic, first, second)
        {
            if (mnemonic.RawName != Mnemonic.Cmp)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Cmp}", nameof(mnemonic));
        }
    }
}