using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic target address</para>
    /// </summary>
    internal abstract class Format2Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format2Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The target operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        public Format2Instruction(Mnemonic mnemonic, TargetOperand operand) : base(mnemonic)
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