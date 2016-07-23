﻿using NSubstitute;
using NUnit.Framework;
using Pote.Text;
using System.Collections.Generic;

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

            Procedure main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(0, main.Instructions.Count);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse the Adc instruction.
        /// </summary>
        [Test]
        public void Parse_AdcInstruction_ParsesInstruction()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Adc(Registers.R3, Registers.R3, 1)
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

            var instruction = main.Instructions[0] as AddWithCarryInstruction;
            Assert.AreEqual(Registers.R3, instruction.FirstOperand.Register);
            Assert.AreEqual(Registers.R3, instruction.Destination.Register);
            Assert.AreEqual(1, instruction.ShifterOperand.Immediate);
            Assert.AreEqual(ShifterOperandType.Immediate, instruction.ShifterOperand.OperandType);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse the Add instruction.
        /// </summary>
        [Test]
        public void Parse_AddInstruction_ParsesInstruction()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Add(Registers.R3, Registers.R3, 1)
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

            var instruction = main.Instructions[0] as AddInstruction;
            Assert.AreEqual(Registers.R3, instruction.FirstOperand.Register);
            Assert.AreEqual(Registers.R3, instruction.Destination.Register);
            Assert.AreEqual(1, instruction.ShifterOperand.Immediate);
            Assert.AreEqual(ShifterOperandType.Immediate, instruction.ShifterOperand.OperandType);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse the B instruction.
        /// </summary>
        [Test]
        public void Parse_BInstruction_ParsesInstruction()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .B(40)
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

            var instruction = main.Instructions[0] as BranchInstruction;
            Assert.AreEqual(40, instruction.Operand.Target);
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