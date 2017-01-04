using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic register</para>
    /// </summary>
    internal abstract class Format4Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format4Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rm">The operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> or <paramref name="rm"/> is null.
        /// </exception>
        protected Format4Instruction(Mnemonic mnemonic, RegisterOperand rm) : base(mnemonic)
        {
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));

            Rm = rm;
        }

        /// <summary>
        /// The operand of the instruction.
        /// </summary>
        public RegisterOperand Rm
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
            return $"{base.ToString()} {Rm}";
        }
    }
}