using Pote.Text;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Lexical analyzer for ArmV6Z assembly programs.
    /// </summary>
    internal class LexicalAnalyzer : LexicalAnalyzer<TokenType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="reader">The source to analyze.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public LexicalAnalyzer(StreamReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="reader">The source to analyze.</param>
        /// <param name="origin">
        /// The origin of the <paramref name="reader"/>.
        /// This is used to give more detailed information if errors occur.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> or <paramref name="origin"/> is null.
        /// </exception>
        public LexicalAnalyzer(StreamReader reader, string origin) : base(reader, origin)
        {
        }

        /// <summary>
        /// Gets the next token from the input.
        /// </summary>
        /// <returns>
        /// The next token from the input. If there is 
        /// no more tokens in the input then a token 
        /// with the type <see cref="TokenType.EndOfInput"/>.
        /// is returned.
        /// </returns>
        /// <remarks>
        /// This method is called by <see cref="LexicalAnalyzer{TokenType}.Next()"/> if there
        /// are no more buffered tokens from looking ahead.
        /// </remarks>
        protected override Token<TokenType> NextTokenFromSource()
        {
            SkipWhitespaces();
            if (Source.EndOfStream)
                return new Token<TokenType>("", TokenType.EndOfInput, Position.DeepCopy());

            return Unknown();
        }

        /// <summary>
        /// Consumes the next unknown from the input.
        /// </summary>
        /// <returns>The consumed unknown from the input.</returns>
        private Token<TokenType> Unknown()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (IsWhitespace(c))
                    break;

                text += Advance();
            }

            return new Token<TokenType>(text, TokenType.Unknown, start);
        }

        /// <summary>
        /// Skips the white spaces in the input.
        /// </summary>
        private void SkipWhitespaces()
        {
            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!IsWhitespace(c))
                    break;

                Advance();
            }
        }

        /// <summary>
        /// Checks if a character is considdered a whitespace.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is considdered a whitespace; otherwise false.</returns>
        private static bool IsWhitespace(char c)
        {
            char[] characters =
            {
                '\u0009', '\u000B', '\u000C', '\u000D',
                '\u000A', '\u0085', '\u2028', '\u2029'
            };

            UnicodeCategory category = char.GetUnicodeCategory(c);

            return category == UnicodeCategory.SpaceSeparator || characters.Contains(c);
        }
    }
}