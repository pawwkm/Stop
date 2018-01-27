using NSubstitute;
using NUnit.Framework;
using Topz.Text;
using System.Collections.Generic;
using System.IO;

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
            var code = new LexicalAnalyzer("procedure main");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer("external procedure main");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, #21");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                add r0, r1, r2");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                add r0, r1, r2, lsl #2");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, lsl r3");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, lsr #2");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, lsr r3");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, asr #2");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, asr r3");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, ror #2");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, ror r3");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 add r0, r1, r2, rrx");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 b #10");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 b Abc");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1, #-1]");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1, +r2]");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1, +r2, lsl #4]");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], 4);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using an immediate pre-indexed offset 12 operand.
        /// </summary>
        /// <remarks>See section A5.2.5.</remarks>
        [Test]
        public void Parse_ImmediatePreIndexedOffset12Operand_ParsesProcedure()
        {
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1, #-1]!");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1, +r2, lsl #4]!");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], 4);
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
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1], #14");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            Assert.AreEqual(instruction.Values[Placeholders.Offset12], 14);
            Assert.Null(instruction.Values[Placeholders.PostIndexed]);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a register post-indexed operand.
        /// </summary>
        /// <remarks>See section A5.2.9.</remarks>
        [Test]
        public void Parse_RegisterPostIndexedOperand_ParsesProcedure()
        {
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1], +r2");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
            Assert.Null(instruction.Values[Placeholders.PostIndexed]);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an instruction using a scaled register post-indexed operand.
        /// </summary>
        /// <remarks>See section A5.2.10.</remarks>
        [Test]
        public void Parse_ScaledRegisterPostIndexedOperand_ParsesProcedure()
        {
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldr r0, [r1], +r2, lsl #4");
            var parser = new Parser();
            var program = parser.Parse(code);

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
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], 4);
            Assert.Null(instruction.Values[Placeholders.PostIndexed]);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse an overloaded instruction.
        /// </summary>
        [Test]
        public void Parse_OverloadedInstruction_ParsesProcedure()
        {
            var code = new LexicalAnalyzer(@"procedure main
                                                 ldrb r0, [r1, +r2, lsl #4]");
            var parser = new Parser();
            var program = parser.Parse(code);

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0, program.Data.Count);
            Assert.AreEqual(0, program.Strings.Count);

            var main = program.Procedures[0];
            Assert.AreEqual("main", main.Name);
            Assert.AreEqual(1, main.Instructions.Count);

            var instruction = main.Instructions[0];
            Assert.AreEqual(instruction.Values.Count, 5);
            Assert.Null(instruction.Label);
            Assert.AreEqual(instruction.Encoding, "cond 01 I P U 1 W 1 Rn Rd addr_mode");
            Assert.AreEqual(instruction.Values[Placeholders.Rd], Register.R0);
            Assert.AreEqual(instruction.Values[Placeholders.Rn], Register.R1);
            Assert.AreEqual(instruction.Values[Symbols.Plus + Placeholders.Rm], Register.R2);
            Assert.AreEqual(instruction.Values[Placeholders.Shift], RegisterShifter.Lsl);
            Assert.AreEqual(instruction.Values[Placeholders.ShiftImmediate], 4);
            Assert.AreEqual(instruction.Mnemonic.Bit, Bit.B);
        }

        /// <summary>
        /// ssafasdf as df asdf 
        /// </summary>
        [Test]
        public void LOL()
        {
            var path = @"C:\Users\Paw\Desktop\Act LED\main.s";
            var assembler = new Assembler();
            var obj = assembler.Assemble(File.ReadAllText(path));
            var linker = new FileFormats.Atom.AtomLinker();

            using (var stream = File.Create(@"C:\Users\Paw\Desktop\Act LED\kernel.img"))
            {
                linker.Link(new[] { obj }, stream);
                stream.Flush();
            }
        }
    }
}