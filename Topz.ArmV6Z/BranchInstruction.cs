using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A branch instruction.
    /// </summary>
    /// <remarks>See A.4.1.5 for more info.</remarks>
    internal class BranchInstruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The target operand of the instruction.</param>
        public BranchInstruction(Label label, Mnemonic mnemonic, TargetOperand operand) : base(label, mnemonic)
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