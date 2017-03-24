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
            builder.Procedure().Identifier("main");

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(0, main.Instructions.Count);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a procedure without any content.
        /// </summary>
        [Test]
        public void Parse_ExternalProcedure_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.External().Procedure().Identifier("main");

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(0, main.Instructions.Count);
            Assert.True(main.IsExternal);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using an immediate data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.3.</remarks>
        [Test]
        public void Parse_ImmediateDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().Integer(21);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 3);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Immediate], 21);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a register data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.4.</remarks>
        [Test]
        public void Parse_RegisterDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 3);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a logical left shift by immediate 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.5.</remarks>
        [Test]
        public void Parse_LogicalLeftShiftByImmediateDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Lsl().Integer(2);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Lsl], 2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a logical left shift by register 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.6.</remarks>
        [Test]
        public void Parse_LogicalLeftShiftByRegisterDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Lsl().R3();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Lsl], Register.R3);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a logical right shift by immediate 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.7.</remarks>
        [Test]
        public void Parse_LogicalRightShiftByImmediateDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Lsr().Integer(2);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Lsr], 2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a logical right shift by register 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.8.</remarks>
        [Test]
        public void Parse_LogicalRightShiftByRegisterDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Lsr().R3();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Lsr], Register.R3);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using an arithmetic right shift by immediate 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.9.</remarks>
        [Test]
        public void Parse_ArithmeticRightShiftByImmediateDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Asr().Integer(2);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Asr], 2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using an arithmetic right shift by register 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.10.</remarks>
        [Test]
        public void Parse_ArithmeticRightShiftByRegisterDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Asr().R3();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Asr], Register.R3);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a right rotation shift by immediate 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.11.</remarks>
        [Test]
        public void Parse_RotateRightShiftByImmediateDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Ror().Integer(2);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Ror], 2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a right rotation shift by register 
        /// data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.12.</remarks>
        [Test]
        public void Parse_RotateRightShiftByRegisterDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Ror().R3();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Ror], Register.R3);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a right rotation with extend data processing operand.
        /// </summary>
        /// <remarks>See section A5.1.13.</remarks>
        [Test]
        public void Parse_RotateRightWithExtendDataProcessingOperand_Success()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic("ADD").R0().Comma().R1().Comma().R2().Comma().Rrx();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 00 I 0100 S Rn Rd shifter_operand");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[RegisterShifter.Rrx], null);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a target address operand.
        /// </summary>
        /// <remarks>See section A4.1.5.</remarks>
        [Test]
        public void Parse_TargetAddressOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.B).Integer(10);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 1);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 101 L signed_immed_24");
            Assert.AreEqual(instruction.Values[Placeholders.TargetAddress], 10);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a target address operand.
        /// </summary>
        /// <remarks>See section A4.1.5.</remarks>
        [Test]
        public void Parse_TargetLabelOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.B).Identifier("Abc");

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 1);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 101 L signed_immed_24");
            Assert.AreEqual(instruction.Values[Placeholders.TargetAddress], "Abc");
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using an immediate offset 12 operand.
        /// </summary>
        /// <remarks>See section A5.2.2.</remarks>
        [Test]
        public void Parse_ImmediateOffset12Operand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().Comma().Integer(-1).RightSquareBracket();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 3);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Offset12], -1);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a register offset operand.
        /// </summary>
        /// <remarks>See section A5.2.3.</remarks>
        [Test]
        public void Parse_RegisterOffsetOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().Comma().Plus().R2().RightSquareBracket();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 3);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a scaled register offset operand.
        /// </summary>
        /// <remarks>See section A5.2.4.</remarks>
        [Test]
        public void Parse_ScaledRegisterOffsetOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().Comma().Plus().R2().Comma().Lsl().Lsr().RightSquareBracket();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 5);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[Placeholders.Shift], RegisterShifter.Lsl);
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], RegisterShifter.Lsr);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using an immediate pre-indexed offset 12 operand.
        /// </summary>
        /// <remarks>See section A5.2.5.</remarks>
        [Test]
        public void Parse_ImmediatePreIndexedOffset12Operand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().Comma().Integer(-1).RightSquareBracket().ExclamationMark();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 4);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Offset12], -1);
            Assert.Null(instruction.Values[Symbols.ExclamationMark]);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a scaled register pre-indexed operand.
        /// </summary>
        /// <remarks>See section A5.2.7.</remarks>
        [Test]
        public void Parse_ScaledRegisterPreIndexedOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().Comma().Plus().R2().Comma().Lsl().Lsr().RightSquareBracket().ExclamationMark();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 6);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[Placeholders.Shift], RegisterShifter.Lsl);
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], RegisterShifter.Lsr);
            Assert.Null(instruction.Values[Symbols.ExclamationMark]);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a immediate 12 post-indexed operand.
        /// </summary>
        /// <remarks>See section A5.2.8.</remarks>
        [Test]
        public void Parse_Immediate12PostIndexedOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().RightSquareBracket().Comma().Integer(14);

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 3);
            Assert.Null(instruction.Label);

            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Placeholders.Offset12], 14);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a register post-indexed operand.
        /// </summary>
        /// <remarks>See section A5.2.9.</remarks>
        [Test]
        public void Parse_RegisterPostIndexedOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().RightSquareBracket().Comma().Plus().R2();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 3);
            Assert.Null(instruction.Label);

            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a scaled register post-indexed operand.
        /// </summary>
        /// <remarks>See section A5.2.10.</remarks>
        [Test]
        public void Parse_ScaledRegisterPostIndexedOperand_ParsesProcedure()
        {
            var builder = new TokenBuilder();
            builder.Procedure().Identifier("main")
                   .Mnemonic(Mnemonic.Ldr).R0().Comma().LeftSquareBracket().R1().RightSquareBracket().Comma().Plus().R2().Comma().Lsl().Lsr();

            var parser = new Parser();
            var program = parser.Parse(LexicalAnalyzer(builder.Build()));

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 5);
            Assert.Null(instruction.Label);

            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 0 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[Placeholders.Shift], RegisterShifter.Lsl);
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], RegisterShifter.Lsr);
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