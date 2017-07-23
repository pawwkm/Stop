using Pote;
using Pote.Text;
using System;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// The registers available in ARMv6Z.
    /// </summary>
    internal sealed class Register : Operand
    {
        /// <summary>
        /// The 'r0' register.
        /// </summary>
        public const string R0 = "r0";

        /// <summary>
        /// The 'r1' register.
        /// </summary>
        public const string R1 = "r1";

        /// <summary>
        /// The 'r2' register.
        /// </summary>
        public const string R2 = "r2";

        /// <summary>
        /// The 'r3' register.
        /// </summary>
        public const string R3 = "r3";

        /// <summary>
        /// The 'r4' register.
        /// </summary>
        public const string R4 = "r4";

        /// <summary>
        /// The 'r5' register.
        /// </summary>
        public const string R5 = "r5";

        /// <summary>
        /// The 'r6' register.
        /// </summary>
        public const string R6 = "r6";

        /// <summary>
        /// The 'r7' register.
        /// </summary>
        public const string R7 = "r7";

        /// <summary>
        /// The 'r8' register.
        /// </summary>
        public const string R8 = "r8";

        /// <summary>
        /// The 'r9' register.
        /// </summary>
        public const string R9 = "r9";

        /// <summary>
        /// The 'r10' register.
        /// </summary>
        public const string R10 = "r10";

        /// <summary>
        /// The 'r11' register.
        /// </summary>
        public const string R11 = "r11";

        /// <summary>
        /// The 'r12' register.
        /// </summary>
        public const string R12 = "r12";

        /// <summary>
        /// The 'r13' register.
        /// </summary>
        public const string R13 = "r13";

        /// <summary>
        /// The 'r14' register.
        /// </summary>
        public const string R14 = "r14";

        /// <summary>
        /// The 'r15' register.
        /// </summary>
        public const string R15 = "r15";

        /// <summary>
        /// The 'sp' register.
        /// </summary>
        public const string StackPointer = "sp";

        /// <summary>
        /// The 'lr' register.
        /// </summary>
        public const string LinkRegister = "lr";

        /// <summary>
        /// The 'pc' register.
        /// </summary>
        public const string ProgramCounter = "pc";

        /// <summary>
        /// Initializes a new instance of the <see cref="Register"/> class.
        /// </summary>
        /// <param name="token">The token that represents a register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="token"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="token"/> is not one of <see cref="All"/>.
        /// The type of <paramref name="token"/> is not <see cref="TokenType.Register"/>.
        /// </exception>
        public Register(Token<TokenType> token) : this(token.Text, token.Position)
        {
            if (token.Type != TokenType.Register)
                throw new ArgumentException("The token is not a register.", nameof(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Register"/> class.
        /// </summary>
        /// <param name="register">The actual register.</param>
        /// <param name="position">The position where the register was referenced.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="register"/> is not one of <see cref="All"/>.
        /// </exception>
        public Register(string register, InputPosition position) : base(position)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (!register.ToLower().IsOneOf(All))
                throw new ArgumentException("This is not a register.", nameof(register));

            Value = register;
        }

        /// <summary>
        /// All of the registers.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new[]
                {
                    R10,
                    R11,
                    R12,
                    R13,
                    R14,
                    R15,
                    R0,
                    R1,
                    R2,
                    R3,
                    R4,
                    R5,
                    R6,
                    R7,
                    R8,
                    R9,
                    StackPointer,
                    LinkRegister,
                    ProgramCounter
                };
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
        public static bool operator ==(Register left, string right)
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
        public static bool operator !=(Register left, string right)
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

            return obj is Register && this == (Register)obj;
        }

        public static explicit operator int (Register register)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register));

            switch (register.Value)
            {
                case R0:
                    return 0;
                case R1:
                    return 1;
                case R2:
                    return 2;
                case R3:
                    return 3;
                case R4:
                    return 4;
                case R5:
                    return 5;
                case R6:
                    return 6;
                case R7:
                    return 7;
                case R8:
                    return 8;
                case R9:
                    return 9;
                case R10:
                    return 10;
                case R11:
                    return 11;
                case R12:
                    return 12;
                case R13:
                case StackPointer:
                    return 13;
                case R14:
                case LinkRegister:
                    return 14;
                case R15:
                case ProgramCounter:
                    return 15;
                default:
                    throw new ArgumentException(nameof(register));
            }
        }

        public static explicit operator uint(Register register)
        {
            return (uint)((int)register);
        }
    }
}