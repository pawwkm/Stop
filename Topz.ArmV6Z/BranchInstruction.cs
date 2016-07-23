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
    }
}