using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic immediate 16</para>
    /// </summary>
    internal abstract class Format3Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format3Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="operand">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="operand"/> is null.
        /// </exception>
        protected Format3Instruction(Mnemonic mnemonic, Immediate16Operand operand) : base(mnemonic)
        {
            if (operand == null)
                throw new ArgumentNullException(nameof(operand));

            Operand = operand;
        }

        /// <summary>
        /// The operand of the instruction.
        /// </summary>
        public Immediate16Operand Operand
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string that represents the current instruction.
        /// </summary>
        /// <returns>A string that represents the current instruction.</returns>
        public override string ToString()
        {
            return $"{base.ToString()} {Operand}";
        }
    }
}