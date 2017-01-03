using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic register, register, shifting operand</para>
    /// </summary>
    internal abstract class Format1Instruction : Instruction
    {
        /// <summary>
        /// Intializes a new instance of the <see cref="Format1Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="r1">The first register.</param>
        /// <param name="r2">The second register.</param>
        /// <param name="shifter">The addressing mode.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="r1"/>, <paramref name="r2"/> or <paramref name="shifter"/> is null.
        /// </exception>
        protected Format1Instruction(Mnemonic mnemonic, Register r1, Register r2, AddressingMode1 shifter) : base(mnemonic)
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
        public Register Destination
        {
            get;
            set;
        }

        /// <summary>
        /// The first operand of the instruction.
        /// </summary>
        public Register FirstOperand
        {
            get;
            set;
        }

        /// <summary>
        /// The second operand of the instruction.
        /// </summary>
        public AddressingMode1 ShifterOperand
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a string that represents the current instruction.
        /// </summary>
        /// <returns>A string that represents the current instruction.</returns>
        public override string ToString()
        {
            return $"{base.ToString()} {Destination}{Symbols.Comma} {FirstOperand}{Symbols.Comma} {ShifterOperand}";
        }
    }
}