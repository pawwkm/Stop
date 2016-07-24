using Pote.Text;

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
        /// Adds the tokens for a <see cref="ArmV6Z.Format1Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="rd">The destination register.</param>
        /// <param name="rn">The register containing the first operand.</param>
        /// <param name="immediate">The value to shift by.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format1Instruction(string mnemonic, string rd, string rn, int immediate)
        {
            return Token(mnemonic, TokenType.Mnemonic)
                  .Token(rd, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Token(rn, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
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
            return Token(mnemonic, TokenType.Mnemonic)
                  .Token("#" + target, TokenType.Integer);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format3Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="target">The target of the branch.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format3Instruction(string mnemonic, ushort target)
        {
            return Token(mnemonic, TokenType.Mnemonic)
                  .Token("#" + target, TokenType.Integer);
        }

        /// <summary>
        /// Adds the tokens for a <see cref="ArmV6Z.Format4Instruction"/> to the builder.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="register">The target of the branch.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Format4Instruction(string mnemonic, string register)
        {
            return Token(mnemonic, TokenType.Mnemonic)
                  .Token(register, TokenType.Register);
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
            return Token(mnemonic, TokenType.Mnemonic)
                  .Token(first, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Token(second, TokenType.Register);
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
            return Token(mnemonic, TokenType.Mnemonic)
                  .Token(register, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Integer(immediate);
        }

        /// <summary>
        /// Adds an integer to the builder.
        /// </summary>
        /// <param name="value">The value of the integer.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Integer(int value)
        {
            return Token("#" + value, TokenType.Integer);
        }
    }
}