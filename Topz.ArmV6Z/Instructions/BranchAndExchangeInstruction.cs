using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Branches to an address, with an optional switch to Thumb state.
    /// </summary>
    /// <remarks>See section A4.1.10</remarks>
    internal sealed class BranchAndExchangeInstruction : Format4Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAndExchangeInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rm">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="rm"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Bx"/>.
        /// </exception>
        public BranchAndExchangeInstruction(Mnemonic mnemonic, RegisterOperand rm) : base(mnemonic, rm)
        {
            if (mnemonic.RawName != Mnemonic.Bx)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Bx}", nameof(mnemonic));
        }
    }
}