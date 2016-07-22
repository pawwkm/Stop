using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Parser for ArmV6Z assembly code.
    /// </summary>
    internal sealed class Parser
    {
        private LexicalAnalyzer<TokenType> analyzer;

        private Program program;

        /// <summary>
        /// Parses a program into an ast.
        /// </summary>
        /// <param name="source">The source to parse.</param>
        /// <returns>The parsed ast.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="ParsingException">
        /// A problem occurred when parsing.
        /// </exception>
        public Program Parse(LexicalAnalyzer<TokenType> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            analyzer = source;

            program = new Program();
            while (!analyzer.EndOfInput)
            {
                if (analyzer.NextIs(Keywords.Procedure))
                    Procedure();
            }

            return program;
        }

        /// <summary>
        /// Parses a procedure.
        /// </summary>
        private void Procedure()
        {
            Token<TokenType> keyword = analyzer.Next();
            if (keyword.Text != Keywords.Procedure)
                throw new ParsingException(keyword.Position.ToString($"Expected the '{Keywords.Procedure}' keyword."));

            Token<TokenType> identifier = analyzer.Next();
            if (identifier.Type != TokenType.Identifier)
                throw new ParsingException(identifier.Position.ToString("Expected an identifier."));

            Token<TokenType> start = analyzer.Next();
            if (start.Text != Symbols.StartOfBlock)
                throw new ParsingException(start.Position.ToString($"Expected the '{Symbols.StartOfBlock}' symbol."));

            Token<TokenType> end = analyzer.Next();
            if (end.Text != Symbols.StartOfBlock)
                throw new ParsingException(end.Position.ToString($"Expected the '{Symbols.EndOfBlock}' symbol."));
        }

        /// <summary>
        /// Parses a piece of data.
        /// </summary>
        private void Data()
        {
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        private void String()
        {
        }
    }
}