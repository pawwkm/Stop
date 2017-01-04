using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// A break point instruction.
    /// </summary>
    /// <remarks>See section A4.1.7</remarks>
    internal sealed class BreakPointInstruction : Format3Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BreakPointInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Bkpt"/>.
        /// </exception>
        public BreakPointInstruction(Mnemonic mnemonic, Immediate16Operand operand) : base(mnemonic, operand)
        {
            if (mnemonic.RawName != Mnemonic.Bkpt)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Bkpt}", nameof(mnemonic));
        }
    }
}