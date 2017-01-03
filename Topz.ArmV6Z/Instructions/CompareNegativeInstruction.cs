using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Compares one value with the twos complement of a second value. The first value 
    /// comes from a register. The second value can be either an immediate value or a value 
    /// from a register, and can be shifted before the comparison.
    /// </summary>
    /// <remarks>See section A4.1.14</remarks>
    internal sealed class CompareNegativeInstruction : Format6Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNegativeInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The shifter operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Cmn"/>.
        /// </exception>
        public CompareNegativeInstruction(Mnemonic mnemonic, Register first, AddressingMode1 second) : base(mnemonic, first, second)
        {
            if (mnemonic.RawName != Mnemonic.Cmn)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Cmn}", nameof(mnemonic));
        }
    }
}