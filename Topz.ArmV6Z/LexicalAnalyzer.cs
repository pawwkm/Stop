using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Pote;
using Topz.Text;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Lexical analyzer for ArmV6Z assembly programs.
    /// </summary>
    internal sealed class LexicalAnalyzer : LexicalAnalyzer<TokenType>
    {
        private static string[] conditions = (from Condition c in Enum.GetValues(typeof(Condition))
                                              where c != ArmV6Z.Condition.Always
                                              select c.AsText()).ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="code">The source code to analyze.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> is null.
        /// </exception>
        public LexicalAnalyzer(string code) : this(code, "")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="code">The source code to analyze.</param>
        /// <param name="path">The path to the <paramref name="code"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> or <paramref name="path"/> is null.
        /// </exception>
        public LexicalAnalyzer(string code, string path) : base(code, path)
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
            if (NoMoreCharacters)
                return new Token<TokenType>("", TokenType.EndOfInput, Position.DeepCopy());

            char c = NextCharacter;
            if (Match(Keywords.All))
                return Keyword();
            if (Match(ArmV6Z.Register.All))
                return Register();
            if (Match(RegisterShifter.All))
                return Shifted();
            if (Match(Symbols.All))
                return Symbol();
            if (Match(Mnemonic.All))
                return LexMnemonic();
            if (Match(conditions))
                return Condition();
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

            while (!NoMoreCharacters)
            {
                char c = NextCharacter;
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
            foreach (string shifter in ArmV6Z.RegisterShifter.All)
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
        /// Consumes the next condition from the input.
        /// </summary>
        /// <returns>The consumed condition from the input.</returns>
        private Token<TokenType> Condition()
        {
            var start = Position.DeepCopy();
            foreach (string condition in conditions)
            {
                if (Consume(condition))
                    return new Token<TokenType>(condition, TokenType.Condition, start);
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

            if (NextCharacter.IsOneOf('-', '+'))
                text += Advance();

            while (!NoMoreCharacters)
            {
                char c = NextCharacter;
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

            while (!NoMoreCharacters)
            {
                if (Consume("\\\""))
                    text += '"';
                else
                {
                    if (NextCharacter == '"')
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
            while (!NoMoreCharacters)
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
            while (!NoMoreCharacters)
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

            while (!NoMoreCharacters)
            {
                if (IsWhitespace(NextCharacter))
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
            while (!NoMoreCharacters)
            {
                if (!IsWhitespace(NextCharacter))
                    break;

                Advance();
            }
        }
    }
}