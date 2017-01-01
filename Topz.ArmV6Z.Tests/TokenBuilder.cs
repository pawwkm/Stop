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
        /// Adds the end of block symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder StartOfBlock()
        {
            return Token(Symbols.StartOfBlock, TokenType.Symbol);
        }

        /// <summary>
        /// Adds the end of block symbol to the builder.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder EndOfBlock()
        {
            return Token(Symbols.EndOfBlock, TokenType.Symbol);
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
        /// Add one of <see cref="Registers.All"/> to the builder.
        /// </summary>
        /// <param name="register">The register to add.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Register(string register)
        {
            if (!Registers.All.Contains(register))
                throw new ArgumentException(nameof(register));

            return Token(register, TokenType.Register);
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
        /// Adds the tokens for a <see cref="ArmV6Z.Format1Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rd">The destination register.</param>
        /// <param name="rn">The register containing the first operand.</param>
        /// <param name="immediate">The value to shift by.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format1Instruction(string mnemonic, string rd, string rn, int immediate)
        {
            return Mnemonic(mnemonic)
                  .Register(rd)
                  .ListItemSeparator()
                  .Register(rn)
                  .ListItemSeparator()
                  .Integer(immediate);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format2Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="target">The target of the branch.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format2Instruction(string mnemonic, int target)
        {
            return Mnemonic(mnemonic)
                  .Integer(target);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format3Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="target">The target of the branch.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format3Instruction(string mnemonic, ushort target)
        {
            return Mnemonic(mnemonic)
                  .Integer(target);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format4Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="register">The target of the branch.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format4Instruction(string mnemonic, string register)
        {
            return Mnemonic(mnemonic)
                  .Register(register);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format5Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="first">The first register operand of the instruction.</param>
        /// <param name="second">The second register operand of the instruction.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format5Instruction(string mnemonic, string first, string second)
        {
            return Mnemonic(mnemonic)
                  .Register(first)
                  .ListItemSeparator()
                  .Register(second);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format6Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="register">The register operand.</param>
        /// <param name="immediate">The value to shift by.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format6Instruction(string mnemonic, string register, int immediate)
        {
            return Mnemonic(mnemonic)
                  .Register(register)
                  .ListItemSeparator()
                  .Integer(immediate);
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

        /// <summary>
        /// Adds the tokens to form the immediate offset addressing mode.
        /// </summary>
        /// <param name="register">The register containing the base address.</param>
        /// <param name="offset">The offset from the base address.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder ImmediateOffsetAddressingMode(string register, int offset)
        {
            return LeftSquareBracket().
                   Register(register).
                   ListItemSeparator().
                   Integer(offset).
                   RightSquareBracket();
        }

        /// <summary>
        /// Adds the tokens to form the immediate offset addressing mode.
        /// </summary>
        /// <param name="baseRegister">The register containing the base address.</param>
        /// <param name="addToBase">If true the offset is added to the base: otherwise it is subtracted.</param>
        /// <param name="offset">The offset from the base address.</param>
        /// <returns></returns>
        public TokenBuilder RegisterOffsetAddressingMode(string baseRegister, bool addToBase, string offset)
        {
            return LeftSquareBracket().
                   Register(baseRegister).
                   ListItemSeparator().
                   Symbol(addToBase ? Symbols.Plus : Symbols.Minus).
                   Register(offset).
                   RightSquareBracket();
        }
    }
}