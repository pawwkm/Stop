using Pote.Text;

namespace Topz.ArmV6Z.Tests
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
        /// Adds the Adc instruction and its operands to the builder.
        /// </summary>
        /// <param name="rd">The destination register.</param>
        /// <param name="rn">The register containing the first operand.</param>
        /// <param name="immediate">The value to shift by.</param>
        /// <returns></returns>
        public TokenBuilder Adc(string rd, string rn, int immediate)
        {
            return Token(Mnemonic.Adc, TokenType.Mnemonic)
                  .Token(rd, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Token(rn, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Integer(immediate);
        }

        /// <summary>
        /// Adds the Add instruction and its operands to the builder.
        /// </summary>
        /// <param name="rd">The destination register.</param>
        /// <param name="rn">The register containing the first operand.</param>
        /// <param name="immediate">The value to shift by.</param>
        /// <returns></returns>
        public TokenBuilder Add(string rd, string rn, int immediate)
        {
            return Token(Mnemonic.Add, TokenType.Mnemonic)
                  .Token(rd, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Token(rn, TokenType.Register)
                  .Token(Symbols.ListItemSeparator, TokenType.Symbol)
                  .Integer(immediate);
        }

        /// <summary>
        /// Adds the B instruction and its operand to the builder.
        /// </summary>
        /// <param name="target">The target of the branch.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder B(int target)
        {
            return Token(Mnemonic.B, TokenType.Mnemonic)
                  .Token("#" + target, TokenType.Integer);
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