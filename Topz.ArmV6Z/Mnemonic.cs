using Pote;
using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a mnemonic in a program.
    /// </summary>
    internal class Mnemonic
    {
        /// <summary>
        /// The 'b' mnemonic.
        /// </summary>
        public const string B = "b";

        /// <summary>
        /// Initializes a new instance of the <see cref="Mnemonic"/> class.
        /// </summary>
        /// <param name="name">The name of the mnemonic.</param>
        /// <param name="position">The position of the mnemonic in the program.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> or <paramref name="position"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is not a mnemonic. Casing is not important.
        /// </exception>
        public Mnemonic(string name, InputPosition position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            if (!name.ToLower().IsOneOf(All.ToArray()))
                throw new ArgumentException("This is not a mnemonic.", nameof(name));

            Name = name;
        }

        /// <summary>
        /// A list of all the mnemonics.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new[]
                {
                    B
                };
            }
        }

        /// <summary>
        /// The name of the mnemonic.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The condition for this instruction.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not valid for this instruction.
        /// </exception>
        public Condition Condition
        {
            get;
            set;
        }

        /// <summary>
        /// The position of the mnemonic in the program.
        /// </summary>
        public InputPosition Position
        {
            get;
            private set;
        }
    }
}