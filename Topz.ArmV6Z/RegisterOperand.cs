using Pote;
using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A register operand.
    /// </summary>
    internal class RegisterOperand
    {
        /// <summary>
        /// Intializes a new instance of the <see cref="RegisterOperand"/> class.
        /// </summary>
        /// <param name="position">The position where the register was referenced.</param>
        /// <param name="register">The actual register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="register"/> is not one of <see cref="Registers.All"/>.
        /// </exception>
        public RegisterOperand(InputPosition position, string register)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (!register.ToLower().IsOneOf(Registers.All))
                throw new ArgumentException("This is not a register.", nameof(register));

            Position = position;
            Register = register;
        }

        /// <summary>
        /// The actual register.
        /// </summary>
        public string Register
        {
            get;
            private set;
        }

        /// <summary>
        /// The position where the register was referenced.
        /// </summary>
        public InputPosition Position
        {
            get;
            private set;
        }
    }
}