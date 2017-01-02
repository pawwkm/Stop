using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Branches to an address and enters the Jazelle state if Jazelle is 
    /// availabel. Otherwise this behaves as <see cref="BranchAndExchangeInstruction"/>.
    /// </summary>
    /// <remarks>See section A4.1.11</remarks>
    internal sealed class BranchAndChangeToJazelleInstruction : Format4Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAndChangeToJazelleInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.Bxj"/>.
        /// </exception>
        public BranchAndChangeToJazelleInstruction(Mnemonic mnemonic, RegisterOperand operand) : base(mnemonic, operand)
        {
            if (mnemonic.RawName != Mnemonic.Bxj)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.Bxj}", nameof(mnemonic));
        }
    }
}