using Pote;
using Topz.Text;
using System;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// The register shifters available in ARMv6Z.
    /// </summary>
    internal sealed class RegisterShifter : Operand
    {
        /// <summary>
        /// The 'asr' shifted register operand.
        /// </summary>
        public const string Asr = "asr";

        /// <summary>
        /// The 'lsl' shifted register operand.
        /// </summary>
        public const string Lsl = "lsl";

        /// <summary>
        /// The 'lsr' shifted register operand.
        /// </summary>
        public const string Lsr = "lsr";

        /// <summary>
        /// The 'ror' shifted register operand.
        /// </summary>
        public const string Ror = "ror";

        /// <summary>
        /// The 'rrx' shifted register operand.
        /// </summary>
        public const string Rrx = "rrx";

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterShifter"/> class.
        /// </summary>
        /// <param name="token">The token that represents a register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="token"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="token"/> is not one of <see cref="All"/>.
        /// The type of <paramref name="token"/> is not <see cref="TokenType.RegisterShifter"/>.
        /// </exception>
        public RegisterShifter(Token<TokenType> token) : this(token.Text, token.Position)
        {
            if (token.Type != TokenType.RegisterShifter)
                throw new ArgumentException("The token is not a register shifter.", nameof(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterShifter"/> class.
        /// </summary>
        /// <param name="shifter">The actual register.</param>
        /// <param name="position">The position where the register was referenced.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="shifter"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="shifter"/> is not one of <see cref="All"/>.
        /// </exception>
        public RegisterShifter(string shifter, Position position) : base(position)
        {
            if (shifter == null)
                throw new ArgumentNullException(nameof(shifter));
            if (!shifter.ToLower().IsOneOf(All))
                throw new ArgumentException("This is not a register shifter.", nameof(shifter));

            Value = shifter;
        }

        /// <summary>
        /// All of the shifted register operands.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                yield return Asr;
                yield return Lsl;
                yield return Lsr;
                yield return Ror;
                yield return Rrx;
            }
        }

        /// <summary>
        /// The actual register.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Compares two registers.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the two registers are the same; otherwise false.</returns>
        public static bool operator ==(RegisterShifter left, string right)
        {
            if ((object)left == null && (object)right == null)
                return true;
            if ((object)left != null && (object)right == null || (object)left == null && (object)right != null)
                return false;

            return left.Value.ToLower() == right.ToLower();
        }

        /// <summary>
        /// Compares two registers.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the two registers are not the same; otherwise false.</returns>
        public static bool operator !=(RegisterShifter left, string right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string that represents the current register.
        /// </summary>
        /// <returns>A string that represents the current register.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Returns the hash code for this register.
        /// </summary>
        /// <returns>The hash code for this register.</returns>
        public override int GetHashCode()
        {
            return Value.ToLower().GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is string)
                return this == (string)obj;

            return obj is RegisterShifter && this == (RegisterShifter)obj;
        }
    }
}
