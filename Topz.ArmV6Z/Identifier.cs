using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// An identifier in a program.
    /// </summary>
    internal sealed class Identifier : Operand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// </summary>
        /// <param name="name">The name of the identifier.</param>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Identifier(string name, InputPosition position) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        /// <summary>
        /// The name of the identifier.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Compares two identifiers.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the two registers are the same; otherwise false.</returns>
        public static bool operator ==(Identifier left, string right)
        {
            if ((object)left == null && (object)right == null)
                return true;
            if ((object)left != null && (object)right == null || (object)left == null && (object)right != null)
                return false;

            return left.Name == right;
        }

        /// <summary>
        /// Compares two string.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the two registers are not the same; otherwise false.</returns>
        public static bool operator !=(Identifier left, string right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for this integer.
        /// </summary>
        /// <returns>The hash code for this integer.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
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

            return obj is Identifier && this == (Identifier)obj;
        }
    }
}