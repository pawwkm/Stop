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
        public void Parse_Format1Instruction_ParsesInstruction(string mnemonic)
        {
            Format1InstructionUsingAnImmediateOperand(mnemonic);
            Format1InstructionUsingARegisterOperand(mnemonic);
            Format1InstructionUsingALogicalLeftShiftByImmediate(mnemonic);
            Format1InstructionUsingALogicalLeftShiftByRegister(mnemonic);
            Format1InstructionUsingALogicalRightShiftByImmediate(mnemonic);
            Format1InstructionUsingALogicalRightShiftByRegister(mnemonic);
            Format1InstructionUsingAnArithmeticRightShiftByImmediate(mnemonic);
            Format1InstructionUsingAnArithmeticRightShiftByRegister(mnemonic);
            Format1InstructionUsingARotateRightByImmediate(mnemonic);
            Format1InstructionUsingARotateRightByRegister(mnemonic);
            Format1InstructionUsingARotateRightWithExtend(mnemonic);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse <see cref="Format2Instruction"/> related instructions.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction to test.</param>
        [Test]
        [TestCase(Mnemonic.B)]
        public void Parse_Format2Instruction_ParsesInstruction(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .Integer(43)
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
            var operand = instruction.Operand as TargetOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(43, operand.Target);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="ImmediateOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingAnImmediateOperand(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R3()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Integer(1)
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
            var operand = instruction.ShifterOperand as ImmediateOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R3, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, instruction.Destination.Value);
            Assert.AreEqual(1, operand.Value);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="RegisterOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingARegisterOperand(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R3()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .R4()
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
            var operand = instruction.ShifterOperand as RegisterOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R3, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, instruction.Destination.Value);
            Assert.AreEqual(Register.R4, operand.Register);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="LogicalLeftShiftByImmediateOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingALogicalLeftShiftByImmediate(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Lsl()
                                .Integer(1)
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
            var operand = instruction.ShifterOperand as LogicalLeftShiftByImmediateOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(1, operand.Shift);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="LogicalLeftShiftByRegisterOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingALogicalLeftShiftByRegister(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Lsl()
                                .R4()
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
            var operand = instruction.ShifterOperand as LogicalLeftShiftByRegisterOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(Register.R4, operand.Shift.Value);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="LogicalRightShiftByImmediateOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingALogicalRightShiftByImmediate(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Lsr()
                                .Integer(1)
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
            var operand = instruction.ShifterOperand as LogicalRightShiftByImmediateOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(1, operand.Shift);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="LogicalRightShiftByRegisterOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingALogicalRightShiftByRegister(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Lsr()
                                .R4()
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
            var operand = instruction.ShifterOperand as LogicalRightShiftByRegisterOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(Register.R4, operand.Shift.Value);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="ArithmeticRightShiftByImmediateOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingAnArithmeticRightShiftByImmediate(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Asr()
                                .Integer(2)
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
            var operand = instruction.ShifterOperand as ArithmeticRightShiftByImmediateOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(2, operand.Shift);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="ArithmeticRightShiftByRegisterOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingAnArithmeticRightShiftByRegister(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Asr()
                                .R4()
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
            var operand = instruction.ShifterOperand as ArithmeticRightShiftByRegisterOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(Register.R4, operand.Shift.Value);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="RotateRightByImmediateOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingARotateRightByImmediate(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Ror()
                                .Integer(2)
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
            var operand = instruction.ShifterOperand as RotateRightByImmediateOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(2, operand.Rotation);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="RotateRightByRegisterOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingARotateRightByRegister(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Ror()
                                .R4()
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
            var operand = instruction.ShifterOperand as RotateRightByRegisterOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
            Assert.AreEqual(Register.R4, operand.Rotation.Value);
        }

        /// <summary>
        /// Tests a given instruction using the <see cref="Format1Instruction"/>
        /// with the <see cref="RotateRightWithExtendOperand"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to use for the test.</param>
        private void Format1InstructionUsingARotateRightWithExtend(string mnemonic)
        {
            var builder = new TokenBuilder();
            var tokens = builder.Procedure()
                                .Identifier("main")
                                .Mnemonic(mnemonic)
                                .R1()
                                .ListItemSeparator()
                                .R2()
                                .ListItemSeparator()
                                .R3()
                                .ListItemSeparator()
                                .Rrx()
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
            var operand = instruction.ShifterOperand as RotateRightWithExtendOperand;

            Assert.AreEqual(mnemonic, instruction.Mnemonic.RawName);
            Assert.AreEqual(Register.R1, instruction.Destination.Value);
            Assert.AreEqual(Register.R2, instruction.FirstOperand.Value);
            Assert.AreEqual(Register.R3, operand.Register.Value);
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