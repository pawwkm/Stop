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
    }
}