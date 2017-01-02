using Pote.Text;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Pote;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Lexical analyzer for ArmV6Z assembly programs.
    /// </summary>
    internal sealed class LexicalAnalyzer : LexicalAnalyzer<TokenType>
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

            char c = (char)Source.Peek();
            if (Source.MatchesAnyOf(Keywords.All))
                return Keyword();
            if (Source.MatchesAnyOf(ArmV6Z.Register.All))
                return Register();
            if (Source.MatchesAnyOf(ArmV6Z.Register.Shifted))
                return Shifted();
            if (Source.MatchesAnyOf(Symbols.All))
                return Symbol();
            if (Source.MatchesAnyOf(false, Mnemonic.All))
                return LexMnemonic();
            if (char.IsLetter(c) || c == '_')
                return Identifier();
            if (c == '#')
                return Integer();
            if (c == '"')
                return String();

            if (c == ';')
            {
                SingleLineComment();
                
                return NextTokenFromSource();
            }
            else if (Consume("/*"))
            {
                MultilineComment();

                return NextTokenFromSource();
            }

            return Unknown();
        }

        /// <summary>
        /// Checks if a character is considered a whitespace.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is considered a whitespace; otherwise false.</returns>
        private static bool IsWhitespace(char c)
        {
            char[] characters =
            {
                '\u0009', '\u000B', '\u000C', '\u000D',
                '\u000A', '\u0085', '\u2028', '\u2029'
            };

            var category = char.GetUnicodeCategory(c);

            return category == UnicodeCategory.SpaceSeparator || characters.Contains(c);
        }

        /// <summary>
        /// Consumes the next mnemonic from the input.
        /// </summary>
        /// <returns>The consumed mnemonic from the input.</returns>
        /// <remarks>
        /// The 'weird' naming is to avoid a name collision between the 
        /// <see cref="Mnemonic"/> class and this method.
        /// </remarks>
        private Token<TokenType> LexMnemonic()
        {
            var start = Position.DeepCopy();
            foreach (string mnemonic in Mnemonic.All)
            {
                if (Consume(mnemonic))
                    return new Token<TokenType>(mnemonic, TokenType.Mnemonic, start);
            }

            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
        }

        /// <summary>
        /// Consumes the next identifier from the input.
        /// </summary>
        /// <returns>The consumed identifier from the input.</returns>
        private Token<TokenType> Identifier()
        {
            var start = Position.DeepCopy();
            var text = "";

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!char.IsLetterOrDigit(c) && !char.IsDigit(c) && c != '_')
                    break;

                text += Advance();
            }

            if (text.Length == 0)
                return Unknown();

            return new Token<TokenType>(text, TokenType.Identifier, start);
        }

        /// <summary>
        /// Consumes the next keyword from the input.
        /// </summary>
        /// <returns>The consumed keyword from the input.</returns>
        private Token<TokenType> Keyword()
        {
            var start = Position.DeepCopy();
            foreach (string keyword in Keywords.All)
            {
                if (Consume(keyword))
                    return new Token<TokenType>(keyword, TokenType.Keyword, start);
            }

            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
        }

        /// <summary>
        /// Consumes the next register from the input.
        /// </summary>
        /// <returns>The consumed register from the input.</returns>
        private Token<TokenType> Register()
        {
            var start = Position.DeepCopy();
            foreach (string register in ArmV6Z.Register.All)
            {
                if (Consume(register))
                    return new Token<TokenType>(register, TokenType.Register, start);
            }

            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
        }

        /// <summary>
        /// Consumes the next register shifter from the input.
        /// </summary>
        /// <returns>The consumed register shifter from the input.</returns>
        private Token<TokenType> Shifted()
        {
            var start = Position.DeepCopy();
            foreach (string shifter in ArmV6Z.Register.Shifted)
            {
                if (Consume(shifter))
                    return new Token<TokenType>(shifter, TokenType.RegisterShifter, start);
            }

            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
        }

        /// <summary>
        /// Consumes the next symbol from the input.
        /// </summary>
        /// <returns>The consumed symbol from the input.</returns>
        private Token<TokenType> Symbol()
        {
            var start = Position.DeepCopy();
            foreach (string symbol in Symbols.All)
            {
                if (Consume(symbol))
                    return new Token<TokenType>(symbol, TokenType.Symbol, start);
            }

            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
        }

        /// <summary>
        /// Consumes the next integer from the input.
        /// </summary>
        /// <returns>The consumed integer from the input.</returns>
        private Token<TokenType> Integer()
        {
            var start = Position.DeepCopy();
            var text = "";

            if (!Consume("#"))
                return Unknown();

            if (Source.Peek().IsOneOf('-', '+'))
                text += Advance();

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!char.IsDigit(c))
                    break;

                text += Advance();
            }

            if (!char.IsDigit(text.Last()))
                return Unknown();

            return new Token<TokenType>(text, TokenType.Integer, start);
        }

        /// <summary>
        /// Consumes the next string from the input.
        /// </summary>
        /// <returns>The consumed string from the input.</returns>
        private Token<TokenType> String()
        {
            var start = Position.DeepCopy();
            var text = "";

            // Skip the first double qoute.
            Advance();

            while (!Source.EndOfStream)
            {
                if (Consume("\\\""))
                    text += '"';
                else
                {
                    char c = (char)Source.Peek();
                    if (c == '"')
                    {
                        // Skip the second double qoute.
                        Advance();

                        break;
                    }

                    text += Advance();
                }
            }

            return new Token<TokenType>(text, TokenType.String, start);
        }

        /// <summary>
        /// Consumes the next single line comment.
        /// </summary>
        private void SingleLineComment()
        {
            // Consume the ';' character.
            Advance();

            var line = Position.Line;
            while (!Source.EndOfStream)
            {
                Advance();
                if (line != Position.Line)
                    break;
            }
        }

        /// <summary>
        /// Consumes the next multi line comment.
        /// </summary>
        private void MultilineComment()
        {
            while (!Source.EndOfStream)
            {
                if (Consume("*/"))
                    break;
                else
                    Advance();
            }
        }

        /// <summary>
        /// Consumes the next unknown from the input.
        /// </summary>
        /// <returns>The consumed unknown from the input.</returns>
        private Token<TokenType> Unknown()
        {
            var start = Position.DeepCopy();
            var text = "";

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
    }
}