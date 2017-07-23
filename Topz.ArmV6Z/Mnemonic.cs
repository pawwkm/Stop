using Pote.Text;
using System;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a mnemonic in a program.
    /// </summary>
    internal sealed class Mnemonic
    {
        /// <summary>
        /// The 'add' mnemonic.
        /// </summary>
        public const string Add = "add";

        /// <summary>
        /// The 'and' mnemonic.
        /// </summary>
        public const string And = "and";

        /// <summary>
        /// The 'b' mnemonic.
        /// </summary>
        public const string B = "b";

        /// <summary>
        /// The 'cmp' mnemonic.
        /// </summary>
        public const string Cmp = "cmp";

        /// <summary>
        /// The 'ldr' mnemonic.
        /// </summary>
        public const string Ldr = "ldr";

        /// <summary>
        /// The 'ldrb' mnemonic.
        /// </summary>
        public const string Ldrb = "ldrb";

        /// <summary>
        /// The 'mov' mnemonic.
        /// </summary>
        public const string Mov = "mov";

        /// <summary>
        /// The 'str' mnemonic.
        /// </summary>
        public const string Str = "str";

        /// <summary>
        /// The 'strh' mnemonic.
        /// </summary>
        public const string Strh = "strh";

        /// <summary>
        /// The 'sub' mnemonic.
        /// </summary>
        public const string Sub = "sub";

        /// <summary>
        /// The 'teq' mnemonic.
        /// </summary>
        public const string Teq = "teq";

        /// <summary>
        /// The 'tst' mnemonic.
        /// </summary>
        public const string Tst = "tst";

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

            Position = position;

            Name = name;
        }

        /// <summary>
        /// A list of all the mnemonics without their extensions.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                yield return Add;
                yield return And;
                yield return B;
                yield return Cmp;
                yield return Ldr;
                yield return Mov;
                yield return Strh;
                yield return Str;
                yield return Sub;
                yield return Teq;
                yield return Tst;
            }
        }
        
        /// <summary>
        /// This is the mnemonic without condition, bits etc.
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
        } = Condition.Always;

        /// <summary>
        /// Specifies if a special bit has been flipped.
        /// </summary>
        public Bit Bit
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

        /// <summary>
        /// Returns a string that represents the current mnemonic.
        /// </summary>
        /// <returns>A string that represents the current mnemonic.</returns>
        public override string ToString()
        {
            return $"{Name}{Condition.AsText()}{Bit.AsText()}";
        }
    }
}