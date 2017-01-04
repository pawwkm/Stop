﻿using Pote;
using Pote.Text;
using System;

namespace Topz.ArmV6Z.Operands
{
    /// <summary>
    /// A register operand.
    /// </summary>
    internal sealed class RegisterOperand : AddressingMode1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterOperand"/> class.
        /// </summary>
        /// <param name="token">The token that represents a register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="token"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="token"/> is not one of <see cref="Register.All"/>.
        /// The type of <paramref name="token"/> is not <see cref="TokenType.Register"/>.
        /// </exception>
        public RegisterOperand(Token<TokenType> token) : this(token.Position, token.Text)
        {
            if (token.Type != TokenType.Register)
                throw new ArgumentException("The token is not a register.", nameof(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterOperand"/> class.
        /// </summary>
        /// <param name="position">The position where the register was referenced.</param>
        /// <param name="register">The actual register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="register"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="register"/> is not one of <see cref="Register.All"/>.
        /// </exception>
        public RegisterOperand(InputPosition position, string register)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (register == null)
                throw new ArgumentNullException(nameof(register));
            if (!register.ToLower().IsOneOf(ArmV6Z.Register.All))
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

        /// <summary>
        /// Returns a string that represents the current register.
        /// </summary>
        /// <returns>A string that represents the current register.</returns>
        public override string ToString()
        {
            return Register;
        }
    }
}