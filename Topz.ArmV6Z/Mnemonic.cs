﻿using Pote.Text;
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
        /// The 'adc' mnemonic.
        /// </summary>
        public const string Adc = "adc";

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
        /// The 'bic' mnemonic.
        /// </summary>
        public const string Bic = "bic";

        /// <summary>
        /// The 'bkpt' mnemonic.
        /// </summary>
        public const string Bkpt = "bkpt";

        /// <summary>
        /// The 'bx' mnemonic.
        /// </summary>
        public const string Bx = "bx";

        /// <summary>
        /// The 'bxj' mnemonic.
        /// </summary>
        public const string Bxj = "bxj";

        /// <summary>
        /// The 'clz' mnemonic.
        /// </summary>
        public const string Clz = "clz";

        /// <summary>
        /// The 'cmn' mnemonic.
        /// </summary>
        public const string Cmn = "cmn";

        /// <summary>
        /// The 'cmp' mnemonic.
        /// </summary>
        public const string Cmp = "cmp";

        /// <summary>
        /// The 'cpy' mnemonic.
        /// </summary>
        public const string Cpy = "cpy";

        /// <summary>
        /// The 'eor' mnemonic.
        /// </summary>
        public const string Eor = "eor";

        /// <summary>
        /// The 'ldr' mnemonic.
        /// </summary>
        public const string Ldr = "ldr";

        private static readonly PreParsedMnemonic[] Table = PreParse(Adc, Bit.S, true).
                                                    Concat(PreParse(Add, Bit.S, true)).
                                                    Concat(PreParse(And, Bit.S, true)).
                                                    Concat(PreParse(B, Bit.L, true)).
                                                    Concat(PreParse(Bic, Bit.S, true)).
                                                    Concat(PreParse(Bkpt, false)).
                                                    Concat(PreParse(Bx)).
                                                    Concat(PreParse(Bxj)).
                                                    Concat(PreParse(Clz)).
                                                    Concat(PreParse(Cmn)).
                                                    Concat(PreParse(Cmp)).
                                                    Concat(PreParse(Cpy)).
                                                    Concat(PreParse(Eor, Bit.S, true)).
                                                    Concat(PreParse(Ldr)).
                                                    OrderByDescending(x => x.Name.Length).ToArray();

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

            var preParsed = (from entry in Table
                             where entry.Name == name.ToLower()
                             select entry).FirstOrDefault();

            if (preParsed == null)
                throw new ArgumentException("This is not a mnemonic.", nameof(name));

            Position = position;

            Name = preParsed.Name;
            RawName = preParsed.RawName;
            Condition = preParsed.Condition;
            Bit = preParsed.Bit;
        }

        /// <summary>
        /// A list of all the mnemonics without their extensions.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return from entry in Table
                       select entry.Name;
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
        /// This is the mnemonic without condition, bits etc.
        /// </summary>
        public string RawName
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
        /// Specifies if a special bit has been flipped.
        /// </summary>
        public Bit Bit
        {
            get;
            private set;
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

        /// <summary>
        /// Creates a list of pre parsed mnemonics for a give mnemonic.
        /// </summary>
        /// <param name="rawName">The name of the mnemonic without conditionals, bits etc.</param>
        /// <param name="includeConditions">
        /// If true the conditions are included; otherwise they are not included.
        /// </param>
        /// <returns>
        /// The list of pre parsed mnemonics. 
        /// The list contains a mnemonic equal to <paramref name="rawName"/>
        /// and one with all the conditionals added.
        /// </returns>
        private static IEnumerable<PreParsedMnemonic> PreParse(string rawName, bool includeConditions = true)
        {
            yield return new PreParsedMnemonic(rawName, rawName, Condition.Always);
            if (includeConditions)
            {
                foreach (Condition condition in typeof(Condition).GetEnumValues())
                    yield return new PreParsedMnemonic(rawName + condition.AsText(), rawName, condition);
            }
        }

        /// <summary>
        /// Creates a list of pre parsed mnemonics for a give mnemonic.
        /// </summary>
        /// <param name="rawName">The name of the mnemonic without conditionals, bits etc.</param>
        /// <param name="bit">The bit the instruction optionally can have.</param>
        /// <param name="bitIsBeforeCondition">If true the bit is before the condition; otherwise it is after the condition.</param>
        /// <returns>
        /// The list of pre parsed mnemonics. 
        /// The list contains a mnemonic equal to <paramref name="rawName"/>, 
        /// one with all the conditionals added, one with bit on and one with a condition and bit on.
        /// </returns>
        private static IEnumerable<PreParsedMnemonic> PreParse(string rawName, Bit bit, bool bitIsBeforeCondition)
        {
            foreach (var value in PreParse(rawName))
                yield return value;

            yield return new PreParsedMnemonic(rawName + bit.AsText(), rawName, Condition.Always, bit);
            foreach (Condition condition in typeof(Condition).GetEnumValues())
            {
                if (bitIsBeforeCondition)
                    yield return new PreParsedMnemonic(rawName + bit.AsText() + condition.AsText(), rawName, condition, bit);
                else
                    yield return new PreParsedMnemonic(rawName + condition.AsText() + bit.AsText(), rawName, condition, bit);
            }
        }

        /// <summary>
        /// Defines the parts of an pre parsed mnemonic.
        /// </summary>
        private class PreParsedMnemonic
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PreParsedMnemonic"/> class.
            /// </summary>
            /// <param name="name">The name of the mnemonic.</param>
            /// <param name="rawName">This is the mnemonic without condition, bits etc.</param>
            /// <param name="condition">The condition for this instruction.</param>
            public PreParsedMnemonic(string name, string rawName, Condition condition)
            {
                Name = name;
                RawName = rawName;
                Condition = condition;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PreParsedMnemonic"/> class.
            /// </summary>
            /// <param name="name">The name of the mnemonic.</param>
            /// <param name="rawName">This is the mnemonic without condition, bits etc.</param>
            /// <param name="condition">The condition for this instruction.</param>
            /// <param name="bit">Specifies if a special bit has been flipped.</param>
            public PreParsedMnemonic(string name, string rawName, Condition condition, Bit bit) : this(name, rawName, condition)
            {
                Bit = bit;
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
            /// This is the mnemonic without condition, bits etc.
            /// </summary>
            public string RawName
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
                private set;
            }

            /// <summary>
            /// Specifies if a special bit has been flipped.
            /// </summary>
            public Bit Bit
            {
                get;
                private set;
            }
        }
    }
}