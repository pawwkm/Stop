using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// A branch instruction.
    /// </summary>
    /// <remarks>See A.4.1.5 for more info.</remarks>
    internal sealed class BranchInstruction : Format2Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchInstruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The target operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mnemonic"/> is not <see cref="Mnemonic.B"/>.
        /// </exception>
        public BranchInstruction(Mnemonic mnemonic, TargetOperand operand) : base(mnemonic, operand)
        {
            if (mnemonic.RawName != Mnemonic.B)
                throw new ArgumentException($"The mnemonic is not {Mnemonic.B}", nameof(mnemonic));
        }
    }
}