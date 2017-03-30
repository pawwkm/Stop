using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// An immediate integer operand.
    /// </summary>
    internal sealed class Integer : Operand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Integer"/> class.
        /// </summary>
        /// <param name="value">The value of the integer.</param>
        /// <param name="position">The position of the integer in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Integer(int value, InputPosition position) : base(position)
        {
            Value = value;
        }

        /// <summary>
        /// The value of the integer.
        /// </summary>
        public int Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Compares two integers.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the two registers are the same; otherwise false.</returns>
        public static bool operator ==(Integer left, int right)
        {
            if ((object)left == null && (object)right == null)
                return true;
            if ((object)left != null && (object)right == null || (object)left == null && (object)right != null)
                return false;

            return left.Value == right;
        }

        /// <summary>
        /// Compares two integers.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the two registers are not the same; otherwise false.</returns>
        public static bool operator !=(Integer left, int right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for this integer.
        /// </summary>
        /// <returns>The hash code for this integer.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is int)
                return this == (int)obj;

            return obj is Integer && this == (Integer)obj;
        }
    }
}