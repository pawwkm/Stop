using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic rd, rm, shifter_operand</para>
    /// </summary>
    internal abstract class Format1Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format1Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rd">The first register.</param>
        /// <param name="rm">The second register.</param>
        /// <param name="shifter">The addressing mode.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="rd"/>, <paramref name="rm"/> or <paramref name="shifter"/> is null.
        /// </exception>
        protected Format1Instruction(Mnemonic mnemonic, Register rd, Register rm, AddressingMode1 shifter) : base(mnemonic)
        {
            if (rd == null)
                throw new ArgumentNullException(nameof(rd));
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));
            if (shifter == null)
                throw new ArgumentNullException(nameof(shifter));

            Destination = rd;
            FirstOperand = rm;
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