﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pote.Text;
using System.Globalization;
using Pote;
using System.IO;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Lexical analyzer for disk scripting.
    /// </summary>
    internal class LexicalAnalyzer : LexicalAnalyzer<TokenType>
    {
        private static readonly string[] Keywords =
        {
            "select", "disk",
            "create", "mbr", "partition", "offset", "sectors"
        };

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
            if (Source.MatchesAnyOf(Keywords))
                return Keyword();
            if (char.IsDigit(c))
                return Integer();
            if (c == '"')
                return String();

            InputPosition start = Position.DeepCopy();
            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
        }

        /// <summary>
        /// Consumes the next keyword from the input.
        /// </summary>
        /// <returns>The consumed keyword from the input.</returns>
        private Token<TokenType> Keyword()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            char c;
            while (!Source.EndOfStream)
            {
                c = (char)Source.Peek();
                if (!char.IsLetter(c))
                    break;

                text += Advance();
            }

            if (!text.ToLower().IsOneOf(Keywords))
                return new Token<TokenType>(text, TokenType.Unknown, start);

            return new Token<TokenType>(text, TokenType.Keyword, start);
        }

        /// <summary>
        /// Consumes the next integer from the input.
        /// </summary>
        /// <returns>The consumed integer from the input.</returns>
        private Token<TokenType> Integer()
        {
            string text = "";
            InputPosition start = Position.DeepCopy();

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!char.IsDigit(c))
                    break;

                text += Advance();
            }

            return new Token<TokenType>(text, TokenType.Integer, start);
        }

        /// <summary>
        /// Consumes the next string from the input.
        /// </summary>
        /// <returns>The consumed string from the input.</returns>
        /// <remarks>
        /// It is assumed that the next character in the input 
        /// a double quote.
        /// </remarks>
        private Token<TokenType> String()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            Advance();
            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (c == '"')
                {
                    Advance();
                    break;
                }

                text += Advance();
            }

            return new Token<TokenType>(text, TokenType.String, start);
        }

        /// <summary>
        /// Skips the white spaces in the input.
        /// </summary>
        private void SkipWhitespaces()
        {
            char[] characters =
            {
                '\u0009', '\u000B', '\u000C', '\u000D',
                '\u000A', '\u0085', '\u2028', '\u2029'
            };

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                UnicodeCategory category = char.GetUnicodeCategory(c);

                if (category != UnicodeCategory.SpaceSeparator && !characters.Contains(c))
                    break;

                Advance();
            }
        }
    }
}