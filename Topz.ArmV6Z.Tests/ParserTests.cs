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
        [TestCase(Mnemonic.Eor)]
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
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format4Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.Bx)]
        [TestCase(Mnemonic.Bxj)]
        public void Parse_Format4Instructions_ParsesInstructions(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Format4Instruction(mnemonic, Registers.R1)
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

            var instruction = main.Instructions[0] as Format4Instruction;
            Assert.AreEqual(Registers.R1, instruction.Operand.Register);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format5Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.Clz)]
        [TestCase(Mnemonic.Cpy)]
        public void Parse_Format5Instructions_ParsesInstructions(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Format5Instruction(mnemonic, Registers.R3, Registers.R4)
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

            var instruction = main.Instructions[0] as Format5Instruction;
            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Registers.R3, instruction.First.Register);
            Assert.AreEqual(Registers.R4, instruction.Second.Register);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format6Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.Cmn)]
        [TestCase(Mnemonic.Cmp)]
        public void Parse_Format6Instructions_ParsesInstructions(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Format6Instruction(mnemonic, Registers.R3, 1)
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

            var instruction = main.Instructions[0] as Format6Instruction;
            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Registers.R3, instruction.First.Register);
            Assert.AreEqual(1, instruction.Second.Immediate);
            Assert.AreEqual(ShifterOperandType.Immediate, instruction.Second.OperandType);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format7Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.Ldr)]
        public void Parse_Format7InstructionWithImmediateOffset_ParsesInstruction(string mnemonic)
        {
            ParseFormat7InstructionWithImmediateOffset(mnemonic);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format7Instruction"/> that uses the immediate offset addressing mode.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        private void ParseFormat7InstructionWithImmediateOffset(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure().Identifier("main")
                                .StartOfBlock()
                                .Mnemonic(mnemonic).Register(Registers.R0)
                                .ListItemSeparator().ImmediateOffsetAddressingMode(Registers.R10, 4)
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

            var instruction = main.Instructions[0] as Format7Instruction;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Registers.R0, instruction.First.Register);
            Assert.AreEqual(Registers.R10, instruction.Second.BaseRegister);
            Assert.AreEqual(4, ((ImmediateOffsetOperand)instruction.Second).Offset);
        }

        /// <summary>
        /// Creates a substitute for an <see cref="LexicalAnalyzer{TokenType}"/>.
        /// </summary>
        /// <param name="tokens">The tokens the analyzer will consume.</param>
        /// <returns>The substitute analyzer.</returns>
        private static LexicalAnalyzer<TokenType> LexicalAnalyzer(IList<Token<TokenType>> tokens)
        {
            var analyzer = Substitute.For<LexicalAnalyzer<TokenType>>();

            var current = 0;
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