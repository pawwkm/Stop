using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic register, addressing mode</para>
    /// </summary>
    /// <remarks>
    /// See section A5.2
    /// </remarks>
    internal abstract class Format7Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format7Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The register operand of the instruction.</param>
        /// <param name="second">The addressing mode operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="first"/> or <paramref name="second"/> is null.
        /// </exception>
        protected Format7Instruction(Mnemonic mnemonic, RegisterOperand first, AddressingMode2 second) : base(mnemonic)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            First = first;
            Second = second;
        }

        /// <summary>
        /// The register operand of the instruction.
        /// </summary>
        public RegisterOperand First
        {
            get;
            private set;
        }

        /// <summary>
        /// The addressing mode operand of the instruction.
        /// </summary>
        public AddressingMode2 Second
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
            return $"{base.ToString()} {First}, {Second}";
        }
    }
}