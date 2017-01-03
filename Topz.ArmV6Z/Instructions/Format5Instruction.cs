using System;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic rd, rm</para>
    /// </summary>
    internal abstract class Format5Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format5Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rd">The first register operand of the instruction.</param>
        /// <param name="rm">The second register operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="rd"/> or <paramref name="rm"/> is null.
        /// </exception>
        protected Format5Instruction(Mnemonic mnemonic, Register rd, Register rm) : base(mnemonic)
        {
            if (rd == null)
                throw new ArgumentNullException(nameof(rd));
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));

            Rd = rd;
            Rm = rm;
        }

        /// <summary>
        /// The first register operand of the instruction.
        /// </summary>
        public Register Rd
        {
            get;
            private set;
        }

        /// <summary>
        /// The second register operand of the instruction.
        /// </summary>
        public Register Rm
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
            return $"{base.ToString()} {Rd}, {Rm}";
        }
    }
}