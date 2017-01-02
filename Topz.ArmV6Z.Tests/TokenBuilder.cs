using Pote.Text;
using System;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Builds tokens for the parser.
    /// </summary>
    internal class TokenBuilder : TokenBuilder<TokenBuilder, TokenType>
    {
        /// <summary>
        /// Adds the 'procedure' keyword to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Procedure()
        {
            return Token(Keywords.Procedure, TokenType.Keyword);
        }

        /// <summary>
        /// Adds an identifier to the stream.
        /// </summary>
        /// <param name="name">The identifier.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Identifier(string name)
        {
            return Token(name, TokenType.Identifier);
        }

        /// <summary>
        /// Add one of <see cref="Symbols.All"/> to the builder.
        /// </summary>
        /// <param name="symbol">The symbol to add.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Symbol(string symbol)
        {
            if (!Symbols.All.Contains(symbol))
                throw new ArgumentException(nameof(symbol));

            return Token(symbol, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the <see cref="Symbols.LeftSquareBracket"/> 
        /// symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder LeftSquareBracket()
        {
            return Token(Symbols.LeftSquareBracket, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the <see cref="Symbols.RightSquareBracket"/> 
        /// symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder RightSquareBracket()
        {
            return Token(Symbols.RightSquareBracket, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the <see cref="Symbols.ListItemSeparator"/> 
        /// symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder ListItemSeparator()
        {
            return Token(Symbols.ListItemSeparator, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the <see cref="Register.R0"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R0()
        {
            return Token(Register.R0, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R1"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R1()
        {
            return Token(Register.R1, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R2"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R2()
        {
            return Token(Register.R2, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R3"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R3()
        {
            return Token(Register.R3, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R4"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R4()
        {
            return Token(Register.R4, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R5"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R5()
        {
            return Token(Register.R5, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R6"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R6()
        {
            return Token(Register.R6, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R7"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R7()
        {
            return Token(Register.R7, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R8"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R8()
        {
            return Token(Register.R8, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R9"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R9()
        {
            return Token(Register.R9, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R10"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R10()
        {
            return Token(Register.R10, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R11"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R11()
        {
            return Token(Register.R11, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R12"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R12()
        {
            return Token(Register.R12, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R13"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R13()
        {
            return Token(Register.R13, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R14"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R14()
        {
            return Token(Register.R14, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.R15"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder R15()
        {
            return Token(Register.R15, TokenType.Register);
        }

        /// <summary>
        /// Adds the <see cref="Register.Asr"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Asr()
        {
            return Token(Register.Asr, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="Register.Lsl"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Lsl()
        {
            return Token(Register.Lsl, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="Register.Lsr"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Lsr()
        {
            return Token(Register.Lsr, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="Register.Ror"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Ror()
        {
            return Token(Register.Ror, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="Register.Rrx"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Rrx()
        {
            return Token(Register.Rrx, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Add one of <see cref="Mnemonic.All"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The register to add.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Mnemonic(string mnemonic)
        {
            if (!ArmV6Z.Mnemonic.All.Contains(mnemonic))
                throw new ArgumentException(nameof(mnemonic));

            return Token(mnemonic, TokenType.Mnemonic);
        }

        /// <summary>
        /// Adds an integer to the builder.
        /// </summary>
        /// <param name="value">The value of the integer.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Integer(int value)
        {
            return Token($"{value}", TokenType.Integer);
        }
    }
}