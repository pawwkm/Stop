using NSubstitute;
using NUnit.Framework;
using Pote.Text;
using System.Collections.Generic;

namespace Topz.ArmV6Z
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

            Procedure main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(0, main.Instructions.Count);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format1Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.Adc)]
        [TestCase(Mnemonic.Add)]
        [TestCase(Mnemonic.And)]
        [TestCase(Mnemonic.Bic)]
        public void Parse_Format1Instructions_ParsesInstructions(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Format1Instruction(mnemonic, Registers.R3, Registers.R3, 1)
                                .EndOfBlock()
                                .Build();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0] as Format1Instruction;
            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Registers.R3, instruction.FirstOperand.Register);
            Assert.AreEqual(Registers.R3, instruction.Destination.Register);
            Assert.AreEqual(1, instruction.ShifterOperand.Immediate);
            Assert.AreEqual(ShifterOperandType.Immediate, instruction.ShifterOperand.OperandType);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format2Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.B)]
        public void Parse_Format2Instructions_ParsesInstructions(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Format2Instruction(mnemonic, 40)
                                .EndOfBlock()
                                .Build();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0] as Format2Instruction;
            Assert.AreEqual(40, instruction.Operand.Target);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format3Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.Bkpt)]
        public void Parse_Format3Instructions_ParsesInstructions(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Format3Instruction(mnemonic, 40)
                                .EndOfBlock()
                                .Build();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0] as Format3Instruction;
            Assert.AreEqual(40, instruction.Operand.Value);
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