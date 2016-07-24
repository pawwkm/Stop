using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic, register, register, shifting operand</para>
    /// </summary>
    internal abstract class Format1Instruction : Instruction
    {
        /// <summary>
        /// Intializes a new instance of the <see cref="Format1Instruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="r1">The first register.</param>
        /// <param name="r2">The second register.</param>
        /// <param name="shifter">The third register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="r1"/>, <paramref name="r2"/> or <paramref name="shifter"/> is null.
        /// </exception>
        protected Format1Instruction(Label label, Mnemonic mnemonic, RegisterOperand r1, RegisterOperand r2, ShifterOperand shifter) : base(label, mnemonic)
        {
            if (r1 == null)
                throw new ArgumentNullException(nameof(r1));
            if (r2 == null)
                throw new ArgumentNullException(nameof(r2));
            if (shifter == null)
                throw new ArgumentNullException(nameof(shifter));

            Destination = r1;
            FirstOperand = r2;
            ShifterOperand = shifter;
        }

        /// <summary>
        /// The destination register.
        /// </summary>
        public RegisterOperand Destination
        {
            get;
            set;
        }

        /// <summary>
        /// The first operand of the instruction.
        /// </summary>
        public RegisterOperand FirstOperand
        {
            get;
            set;
        }

        /// <summary>
        /// The second operand of the instruction.
        /// </summary>
        public ShifterOperand ShifterOperand
        {
            get;
            set;
        }
    }
}