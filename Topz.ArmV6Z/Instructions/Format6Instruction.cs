using System;
using Topz.ArmV6Z.Operands;

namespace Topz.ArmV6Z.Instructions
{
    /// <summary>
    /// <para>An instruction with the following syntax.</para>
    /// <para>mnemonic Rn, shifter operand</para>
    /// </summary>
    internal abstract class Format6Instruction : Instruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format6Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rn">The register operand of the instruction.</param>
        /// <param name="shifter">The shifter operand of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="rn"/> or <paramref name="shifter"/> is null.
        /// </exception>
        protected Format6Instruction(Mnemonic mnemonic, Register rn, AddressingMode1 shifter) : base(mnemonic)
        {
            if (rn == null)
                throw new ArgumentNullException(nameof(rn));
            if (shifter == null)
                throw new ArgumentNullException(nameof(shifter));

            Rn = rn;
            Shifter = shifter;
        }

        /// <summary>
        /// The register operand of the instruction.
        /// </summary>
        public Register Rn
        {
            get;
            private set;
        }

        /// <summary>
        /// The shifter operand of the instruction.
        /// </summary>
        public AddressingMode1 Shifter
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
            return $"{base.ToString()} {Rn}, {Shifter}";
        }
    }
}