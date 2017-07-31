using NUnit.Framework;
using Pote.Text;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// provides tests for the <see cref="Encoder"/> class.
    /// </summary>
    internal sealed class EncoderTests
    {
        /// <summary>
        /// Tests that <see cref="Encoder.Encode(ref uint, int, Condition)"/>
        /// can encode a condition at a given index in the instruction.
        /// </summary>
        /// <param name="condition">The condition to encode.</param>
        [TestCase(Condition.Always)]
        [TestCase(Condition.CarryClear)]
        [TestCase(Condition.CarrySet)]
        [TestCase(Condition.Equal)]
        [TestCase(Condition.LessThanOrEqual)]
        [TestCase(Condition.Minus)]
        [TestCase(Condition.NoOverflow)]
        [TestCase(Condition.NotEqual)]
        [TestCase(Condition.Overflow)]
        [TestCase(Condition.Plus)]
        [TestCase(Condition.SignedGreaterThan)]
        [TestCase(Condition.SignedGreaterThanOrEqual)]
        [TestCase(Condition.SignedLessThan)]
        [TestCase(Condition.UnsignedHigher)]
        [TestCase(Condition.UnsignedLowerOrSame)]
        public void Encode_Condition_CorrectlyEncoded(Condition condition)
        {
            var instruction = 0u;
            var index = 31;

            index = Encoder.Encode(ref instruction, index, condition);

            Assert.AreEqual((uint)condition, instruction);
            Assert.AreEqual(27, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.Encode(ref uint, int, Register)"/>
        /// can encode a register at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register to encode.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R0, 0u)]
        [TestCase(Register.R1, 1u)]
        [TestCase(Register.R2, 2u)]
        [TestCase(Register.R3, 3u)]
        [TestCase(Register.R4, 4u)]
        [TestCase(Register.R5, 5u)]
        [TestCase(Register.R6, 6u)]
        [TestCase(Register.R7, 7u)]
        [TestCase(Register.R8, 8u)]
        [TestCase(Register.R9, 9u)]
        [TestCase(Register.R10, 10u)]
        [TestCase(Register.R11, 11u)]
        [TestCase(Register.R12, 12u)]
        [TestCase(Register.R13, 13u)]
        [TestCase(Register.R14, 14u)]
        [TestCase(Register.R15, 15u)]
        [TestCase(Register.StackPointer, 13u)]
        [TestCase(Register.LinkRegister, 14u)]
        [TestCase(Register.ProgramCounter, 15u)]
        public void Encode_Register_CorectlyEncoded(string register, uint expected)
        {
            var r = new Register(register, new InputPosition());
            var instruction = 0u;
            var index = 3;

            index = Encoder.Encode(ref instruction, index, r);

            Assert.AreEqual(expected, instruction);
            Assert.AreEqual(-1, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.Encode(ref uint, int, string)"/>
        /// can encode a set of bits at a given index in the instruction.
        /// </summary>
        [Test]
        public void Encode_Bits_CorectlyEncoded()
        {
            var bits = "1011";
            var instruction = 0u;
            var index = 3;

            index = Encoder.Encode(ref instruction, index, bits);

            Assert.AreEqual(11u, instruction);
            Assert.AreEqual(-1, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.Encode(ref uint, int, bool)"/>
        /// can encode a bit at a given index in the instruction.
        /// </summary>
        [Test]
        public void Encode_Bit_CorectlyEncoded()
        {
            var instruction = 0u;
            var index = 3;

            index = Encoder.Encode(ref instruction, index, true);

            Assert.AreEqual(8u, instruction);
            Assert.AreEqual(2, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a register at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register to encode.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R0, 0u)]
        [TestCase(Register.R1, 1u)]
        [TestCase(Register.R2, 2u)]
        [TestCase(Register.R3, 3u)]
        [TestCase(Register.R4, 4u)]
        [TestCase(Register.R5, 5u)]
        [TestCase(Register.R6, 6u)]
        [TestCase(Register.R7, 7u)]
        [TestCase(Register.R8, 8u)]
        [TestCase(Register.R9, 9u)]
        [TestCase(Register.R10, 10u)]
        [TestCase(Register.R11, 11u)]
        [TestCase(Register.R12, 12u)]
        [TestCase(Register.R13, 13u)]
        [TestCase(Register.R14, 14u)]
        [TestCase(Register.R15, 15u)]
        [TestCase(Register.StackPointer, 13u)]
        [TestCase(Register.LinkRegister, 14u)]
        [TestCase(Register.ProgramCounter, 15u)]
        public void EncodeShifterOperand_RegisterOperand_CorectlyEncoded(string register, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(register, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a immediate at a given index in the instruction.
        /// </summary>
        /// <param name="immediate">The immediate to encode.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(0, 0u)]
        [TestCase(21, 0b0000_0001_0101u)]
        [TestCase(0x0003FC00, 0b1011_1111_1111u)]
        public void EncodeShifterOperand_ImmediateOperand_CorectlyEncoded(int immediate, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Immediate, new Integer(immediate, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a logical left shift by immediate data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="shift">The immediate to shift by.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R2, 2, 0x102u)]
        public void EncodeShifterOperand_LogicalLeftShiftByImmediateDataProcessingOperand_CorectlyEncoded(string register, int shift, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(register, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Lsl, new Integer(shift, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a logical left shift by register data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R15, 0xF12u)]
        public void EncodeShifterOperand_LogicalLeftShiftByRegisterDataProcessingOperand_CorectlyEncoded(string register, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(Register.R2, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Lsl, new Register(register, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a logical right shift by immediate data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="shift">The immediate to shift by.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R2, 2, 0x122u)]
        public void EncodeShifterOperand_LogicalRightShiftByImmediateDataProcessingOperand_CorectlyEncoded(string register, int shift, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(register, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Lsr, new Integer(shift, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a logical right shift by register data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R15, 0xF32u)]
        public void EncodeShifterOperand_LogicalRightShiftByRegisterDataProcessingOperand_CorectlyEncoded(string register, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(Register.R2, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Lsr, new Register(register, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode an arithmetic shift right by immediate data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="shift">The immediate to shift by.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R2, 2, 0x142u)]
        public void EncodeShifterOperand_ArithmeticRightShiftByImmediateDataProcessingOperand_CorectlyEncoded(string register, int shift, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(register, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Asr, new Integer(shift, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode an arithmetic shift right by register data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R15, 0xF52u)]
        public void EncodeShifterOperand_ArithmeticRightShiftByRegisterDataProcessingOperand_CorectlyEncoded(string register, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(Register.R2, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Asr, new Register(register, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }
        
        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a rotate right shift by immediate data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="shift">The immediate to shift by.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R2, 2, 0x162u)]
        public void EncodeShifterOperand_RotateRightShiftByImmediateDataProcessingOperand_CorectlyEncoded(string register, int shift, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(register, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Ror, new Integer(shift, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a rotate right shift by register data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R15, 0xF72u)]
        public void EncodeShifterOperand_RotateRightShiftByRegisterDataProcessingOperand_CorectlyEncoded(string register, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(Register.R2, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Ror, new Register(register, new InputPosition()));

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeShifterOperand(ref uint, int, Instruction)"/>
        /// can encode a rotate right with extend data processing
        /// operand at a given index in the instruction.
        /// </summary>
        /// <param name="register">The register in the shift.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(Register.R0, 0x60u)]
        [TestCase(Register.R1, 0x61u)]
        [TestCase(Register.R2, 0x62u)]
        [TestCase(Register.R3, 0x63u)]
        [TestCase(Register.R4, 0x64u)]
        [TestCase(Register.R5, 0x65u)]
        [TestCase(Register.R6, 0x66u)]
        [TestCase(Register.R7, 0x67u)]
        [TestCase(Register.R8, 0x68u)]
        [TestCase(Register.R9, 0x69u)]
        [TestCase(Register.R10, 0x6Au)]
        [TestCase(Register.R11, 0x6Bu)]
        [TestCase(Register.R12, 0x6Cu)]
        [TestCase(Register.R13, 0x6Du)]
        [TestCase(Register.R14, 0x6Eu)]
        [TestCase(Register.R15, 0x6Fu)]
        [TestCase(Register.StackPointer, 0x6Du)]
        [TestCase(Register.LinkRegister, 0x6Eu)]
        [TestCase(Register.ProgramCounter, 0x6Fu)]
        public void EncodeShifterOperand_RotateRightWithExtendDataProcessingOperand_CorectlyEncoded(string register, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Rm, new Register(register, new InputPosition()));
            instruction.Values.Add(RegisterShifter.Rrx, null);

            index = Encoder.EncodeShifterOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeLoadStoreOperand(ref uint, int, Instruction)"/> 
        /// can encode a immediate load/store operand at a given index in the instruction.
        /// </summary>
        /// <param name="offset">The immediate offset.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(-4095, 0x5000FFFu)]
        [TestCase(0, 0x5000000u)]
        [TestCase(4095, 0x5000FFFu)]
        public void EncodeLoadStoreOperand_ImmediateOffset12Operand_CorectlyEncoded(int offset, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.Offset12, new Integer(offset, new InputPosition()));

            index = Encoder.EncodeLoadStoreOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        /// Tests that <see cref="Encoder.EncodeLoadStoreOperand(ref uint, int, Instruction)"/> 
        /// can encode a scaled register offset operand at a given index in the instruction.
        /// </summary>
        /// <param name="shifter">The register shifter.</param>
        /// <param name="offset">The immediate offset.</param>
        /// <param name="expected">The expected result of the encoding.</param>
        [TestCase(RegisterShifter.Lsl, 0, 0x7800007u)]
        [TestCase(RegisterShifter.Lsl, 31, 0x7800F87u)]
        [TestCase(RegisterShifter.Lsr, 1, 0x78000A7u)]
        [TestCase(RegisterShifter.Lsr, 16, 0x7800827u)]
        [TestCase(RegisterShifter.Lsr, 32, 0x7800027u)]
        [TestCase(RegisterShifter.Asr, 1, 0x78000C7u)]
        [TestCase(RegisterShifter.Asr, 16, 0x7800847u)]
        [TestCase(RegisterShifter.Asr, 32, 0x7800047u)]
        [TestCase(RegisterShifter.Ror, 1, 0x78000E7u)]
        [TestCase(RegisterShifter.Ror, 16, 0x7800867u)]
        [TestCase(RegisterShifter.Ror, 31, 0x7800FE7u)]
        [TestCase(RegisterShifter.Rrx, 0, 0x7800067u)]
        public void EncodeLoadStoreOperand_ScaledRegisterOffsetOperand_CorectlyEncoded(string shifter, int offset, uint expected)
        {
            var binary = 0u;
            var index = 11;

            var instruction = new Instruction(new Mnemonic(Mnemonic.Add, new InputPosition()));
            instruction.Values.Add(Placeholders.ShiftImmediate, new Integer(offset, new InputPosition()));
            instruction.Values.Add(Placeholders.Shift, new RegisterShifter(shifter, new InputPosition()));
            instruction.Values.Add(Placeholders.Rn, new Register(Register.R6, new InputPosition()));
            instruction.Values.Add(Symbols.Plus + Placeholders.Rm, new Register(Register.R7, new InputPosition()));

            index = Encoder.EncodeLoadStoreOperand(ref binary, index, instruction);

            Assert.AreEqual(expected, binary);
            Assert.AreEqual(0, index);
        }
    }
}