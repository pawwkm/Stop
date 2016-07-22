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
    internal sealed class Mnemonic
    {
        /// <summary>
        /// The 'b' mnemonic.
        /// </summary>
        public const string B = "b";

        /// <summary>
        /// The 'eq' mnemonic extension.
        /// </summary>
        public const string EqualExtension = "eq";

        /// <summary>
        /// The 'ne' mnemonic extension.
        /// </summary>
        public const string NotEqualExtension = "ne";

        /// <summary>
        /// The 'cs' mnemonic extension.
        /// </summary>
        public const string CarrySetExtension = "cs";

        /// <summary>
        /// The 'hs' mnemonic extension.
        /// </summary>
        public const string UnsignedHigherOrSameExtension = "hs";

        /// <summary>
        /// The 'cc' mnemonic extension.
        /// </summary>
        public const string CarryClearExtension = "cc";

        /// <summary>
        /// The 'cl' mnemonic extension.
        /// </summary>
        public const string UnsignedLowerExtension = "cl";

        /// <summary>
        /// The 'mi' mnemonic extension.
        /// </summary>
        public const string MinusExtension = "mi";

        /// <summary>
        /// The 'pl' mnemonic extension.
        /// </summary>
        public const string PlusExtension = "pl";

        /// <summary>
        /// The 'vs' mnemonic extension.
        /// </summary>
        public const string OverflowExtension = "vs";

        /// <summary>
        /// The 'vc' mnemonic extension.
        /// </summary>
        public const string NoOverflowExtension = "vc";

        /// <summary>
        /// The 'hi' mnemonic extension.
        /// </summary>
        public const string UnsignedHigherExtension = "hi";

        /// <summary>
        /// The 'ls' mnemonic extension.
        /// </summary>
        public const string UnsignedLowerOrSameExtension = "ls";

        /// <summary>
        /// The 'ge' mnemonic extension.
        /// </summary>
        public const string SignedGreaterThanOrEqualExtension = "ge";

        /// <summary>
        /// The 'lt' mnemonic extension.
        /// </summary>
        public const string SignedLessThanExtension = "lt";

        /// <summary>
        /// The 'gt' mnemonic extension.
        /// </summary>
        public const string SignedGreaterThanExtension = "gt";

        /// <summary>
        /// The 'le' mnemonic extension.
        /// </summary>
        public const string LessThanOrEqualExtension = "le";

        /// <summary>
        /// The 'al' mnemonic extension.
        /// </summary>
        public const string AlwaysExtension = "al";

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
        /// A list of all the mnemonics without their extensions.
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
        /// A list of all the mnemonics with their extensions.
        /// </summary>
        public static IEnumerable<string> AllWithExtensions
        {
            get
            {
                foreach (string mnemonic in All)
                {

                    foreach (string extension in CommonExtensions)
                        yield return mnemonic + extension;
                }
            }
        }

        /// <summary>
        /// A list of all the mnemonics with and without their extensions.
        /// Essentially a combo of <see cref="All"/> and <see cref="AllWithExtensions"/>.
        /// </summary>
        public static IEnumerable<string> AllWithAndWithoutExtensions
        {
            get
            {
                foreach (var mnemonic in All)
                    yield return mnemonic;

                foreach (var mnemonic in AllWithExtensions)
                    yield return mnemonic;
            }
        }

        /// <summary>
        /// All common extensions.
        /// </summary>
        public static IEnumerable<string> CommonExtensions
        {
            get
            {
                return new string[]
                {
                    EqualExtension,
                    NotEqualExtension,
                    CarrySetExtension,
                    UnsignedHigherOrSameExtension,
                    CarryClearExtension,
                    UnsignedLowerExtension,
                    MinusExtension,
                    PlusExtension,
                    OverflowExtension,
                    NoOverflowExtension,
                    UnsignedHigherExtension,
                    UnsignedLowerOrSameExtension,
                    SignedGreaterThanOrEqualExtension,
                    SignedLessThanExtension,
                    SignedGreaterThanExtension,
                    LessThanOrEqualExtension,
                    AlwaysExtension
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
        /// The same as <see cref="Name"/>, but without the condition at the end.
        /// </summary>
        public string NameWithoutCondtition
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