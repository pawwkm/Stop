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
        /// Adds the 'external' keyword to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder External()
        {
            return Token(Keywords.External, TokenType.Keyword);
        }

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
        /// Adds the <see cref="Symbols.ExclamationMark"/> 
        /// symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder ExclamationMark()
        {
            return Token(Symbols.ExclamationMark, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the <see cref="Symbols.Comma"/> 
        /// symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Comma()
        {
            return Token(Symbols.Comma, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the <see cref="Symbols.Plus"/> 
        /// symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Plus()
        {
            return Token(Symbols.Plus, TokenType.Symbol);
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
        /// Adds the <see cref="RegisterShifter.Asr"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Asr()
        {
            return Token(RegisterShifter.Asr, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="RegisterShifter.Lsl"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Lsl()
        {
            return Token(RegisterShifter.Lsl, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="RegisterShifter.Lsr"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Lsr()
        {
            return Token(RegisterShifter.Lsr, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="RegisterShifter.Ror"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Ror()
        {
            return Token(RegisterShifter.Ror, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Adds the <see cref="RegisterShifter.Rrx"/> to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Rrx()
        {
            return Token(RegisterShifter.Rrx, TokenType.RegisterShifter);
        }

        /// <summary>
        /// Add one of <see cref="Mnemonic.All"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The register to add.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Mnemonic(string mnemonic)
        {
            return Token(mnemonic.ToLower(), TokenType.Mnemonic);
        }

        /// <summary>
        /// Adds a condition to an instruction.
        /// </summary>
        /// <param name="condition">The condition to add.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Condition(Condition condition)
        {
            return Token(condition.AsText(), TokenType.Condition);
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