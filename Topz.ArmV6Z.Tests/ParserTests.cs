using NSubstitute;
using NUnit.Framework;
using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topz.ArmV6Z.Tests
{
    /// <summary>
    /// Provides tests for the <see cref="Parser"/> class.
    /// </summary>
    [TestFixture]
    public class ParserTests
    {
        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a procedure without any content.
        /// </summary>
        [Test]
        public void Parse_EmptyProcedure_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .EndOfBlock()
                                .Build();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);
        }

        /// <summary>
        /// Creates a substitute for an <see cref="LexicalAnalyzer{TokenType}"/>.
        /// </summary>
        /// <param name="tokens">The tokens the analyzer will consume.</param>
        /// <returns>The substitute analyzer.</returns>
        private static LexicalAnalyzer<TokenType> LexicalAnalyzer(IList<Token<TokenType>> tokens)
        {
            LexicalAnalyzer<TokenType> analyzer = Substitute.For<LexicalAnalyzer<TokenType>>();

            int current = 0;
            analyzer.Next().Returns(x =>
            {
                if (analyzer.EndOfInput)
                    return new Token<TokenType>("", TokenType.EndOfInput, new InputPosition());

                return tokens[current++];
            });

            analyzer.LookAhead(Arg.Any<int>()).Returns(x =>
            {
                if (current + x.Arg<int>() - 1 == tokens.Count)
                    return new Token<TokenType>("", TokenType.EndOfInput, new InputPosition());

                return tokens[current + x.Arg<int>() - 1];
            });

            analyzer.EndOfInput.Returns(x => current == tokens.Count);

            return analyzer;
        }
    }
}
