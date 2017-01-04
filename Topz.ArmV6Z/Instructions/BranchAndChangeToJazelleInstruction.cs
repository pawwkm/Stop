using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// Branches to an address and enters the Jazelle state if Jazelle is 
    /// available. Otherwise this behaves as <see cref="BranchAndExchangeInstruction"/>.
    /// </summary>
    /// <remarks>See section A4.1.11</remarks>
    internal sealed class BranchAndChangeToJazelleInstruction : Format4Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAndChangeToJazelleInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rm">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="rm"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Bxj"/>.
        /// </exception>
        public BranchAndChangeToJazelleInstruction(Mnemonic mnemonic, RegisterOperand rm) : base(mnemonic, rm)
        {
            if (mnemonic.RawName != Mnemonic.Bxj)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Bxj}", nameof(mnemonic));
        }
    }
}