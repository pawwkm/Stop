using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents an instruction in a program.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    internal sealed class Instruction : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/> is null.
        /// </exception>
        public Instruction(Mnemonic mnemonic) : base(mnemonic.Position)
        {
            if (mnemonic == null)
                throw new ArgumentNullException(nameof(mnemonic));

            Mnemonic = mnemonic;
        }

        /// <summary>
        /// The label of the instruction. 
        /// If null, this instruction doesn't have any label.
        /// </summary>
        public Label Label
        {
            get;
            set;
        }

        /// <summary>
        /// The mnemonic of the instruction.
        /// </summary>
        public Mnemonic Mnemonic
        {
            get;
            private set;
        }

        /// <summary>
        /// The textual format of the instruction.
        /// </summary>
        public string Format
        {
            get;
            set;
        }

        /// <summary>
        /// The encoding of the instruction.
        /// </summary>
        public string Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// The instruction values to encode.
        /// </summary>
        public IDictionary<string, Operand> Values
        {
            get;
        } = new Dictionary<string, Operand>();

        /// <summary>
        /// Returns a string that represents the current instruction.
        /// </summary>
        /// <returns>A string that represents the current instruction.</returns>
        public override string ToString()
        {
            if (Label == null)
                return Mnemonic.ToString();

            return $"{Label.Name}: {Mnemonic}";
        }
    }
}