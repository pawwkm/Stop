using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A branch instruction.
    /// </summary>
    internal class BranchInstruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="position">The position of the instruction in the program's source code.</param>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The target operand of the instruction.</param>
        public BranchInstruction(InputPosition position, Label label, Mnemonic mnemonic, TargetOperand operand) : base(position, label, mnemonic)
        {
            if (operand == null)
                throw new ArgumentNullException(nameof(operand));

            Operand = operand;
        }

        /// <summary>
        /// The address to branch to.
        /// </summary>
        public TargetOperand Operand
        {
            get;
            private set;
        }

        /// <summary>
        /// If true the L bit (bit 24) in the instruction to be set to 1. The resulting instruction stores a 
        /// return address in the link register (R14). If false, the instruction simply branches without 
        /// storing a return address.
        /// </summary>
        public bool BranchAndLink
        {
            get;
        }
    }
}