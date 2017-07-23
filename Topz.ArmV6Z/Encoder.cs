using System;

namespace Topz.ArmV6Z
{
    internal static class Encoder
    {
        /// <summary>
        /// Encodes a condition into an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to apply the condition to.</param>
        /// <param name="index">The index where the condition is applied to.</param>
        /// <param name="condition">The condition to apply.</param>
        /// <returns>The new index.</returns>
        public static int Encode(ref uint instruction, int index, Condition condition)
        {
            instruction |= (uint)condition;

            return index - 4;
        }

        /// <summary>
        /// Encodes a register into an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to apply the register to.</param>
        /// <param name="index">The index where the register is applied to.</param>
        /// <param name="register">The register to apply.</param>
        /// <returns>The new index.</returns>
        public static int Encode(ref uint instruction, int index, Register register)
        {
            instruction |= (uint)register << index - 3;

            return index - 4;
        }

        /// <summary>
        /// Encodes a set of bits into an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to apply the bits to.</param>
        /// <param name="index">The index where the bits are applied to.</param>
        /// <param name="bits">The bits to apply.</param>
        /// <returns>The new index.</returns>
        public static int Encode(ref uint instruction, int index, string bits)
        {
            foreach (var bit in bits)
                instruction = instruction.SetBit((byte)index--, bit == '1');

            return index;
        }

        /// <summary>
        /// Encodes a bit into an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to apply the bit to.</param>
        /// <param name="index">The index where the bit is applied to.</param>
        /// <param name="bit">The bit to apply.</param>
        /// <returns>The new index.</returns>
        public static int Encode(ref uint instruction, int index, bool bit)
        {
            instruction = instruction.SetBit((byte)index--, bit);

            return index;
        }

        /// <summary>
        /// Encodes a bit into an instruction.
        /// </summary>
        /// <param name="binary">The instruction to apply the bit to.</param>
        /// <param name="index">The index where the bit is applied to.</param>
        /// <param name="instruction">The instruction that carries the shifter operand.</param>
        /// <returns>The new index.</returns>
        public static int EncodeShifterOperand(ref uint binary, int index, Instruction instruction)
        {
            if (instruction.Values.ContainsKey(Placeholders.Immediate))
            {
                var v = (uint)((Integer)instruction.Values[Placeholders.Immediate]).Value;
                binary |= (Encode(v) << index - 11);
            }
            else if (instruction.Values.ContainsKey(Placeholders.Rm))
            {
                var rm = (uint)((Register)instruction.Values[Placeholders.Rm]);
                if (instruction.Values.ContainsKey(RegisterShifter.Lsl))
                {
                    if (instruction.Values[RegisterShifter.Lsl] is Integer)
                    {
                        var immediate = (uint)((Integer)instruction.Values[RegisterShifter.Lsl]).Value;
                        if (immediate > 31 || immediate < 0)
                            throw new EncodingException(instruction.Values[RegisterShifter.Lsl].Position.ToString("The shift must be between 0 and 31."));

                        binary |= immediate << index - 4;
                    }
                    else if (instruction.Values[RegisterShifter.Lsl] is Register)
                    {
                        var rs = (Register)instruction.Values[RegisterShifter.Lsl];
                        binary |= (uint)rs << index - 3;
                        binary |= 1u << 4; // Set the 4th bit.
                    }
                    else
                        throw new EncodingException(instruction.Mnemonic.Position.ToString("Expected a register or shift."));
                }
                else if (instruction.Values.ContainsKey(RegisterShifter.Lsr))
                {
                    if (instruction.Values[RegisterShifter.Lsr] is Integer)
                    {
                        var immediate = (uint)((Integer)instruction.Values[RegisterShifter.Lsr]).Value;
                        if (immediate > 31 || immediate < 0)
                            throw new EncodingException(instruction.Values[RegisterShifter.Lsr].Position.ToString("The shift must be between 0 and 31."));

                        binary |= immediate << index - 4;
                        binary |= 1u << 5; // Set the 5th bit.
                    }
                    else if (instruction.Values[RegisterShifter.Lsr] is Register)
                    {
                        var rs = (Register)instruction.Values[RegisterShifter.Lsr];
                        binary |= (uint)rs << index - 3;
                        binary |= 1u << 5; // Set the 5th bit.
                        binary |= 1u << 4; // Set the 4th bit.
                    }
                    else
                        throw new EncodingException(instruction.Mnemonic.Position.ToString("Expected a register or shift."));
                }
                else if (instruction.Values.ContainsKey(RegisterShifter.Asr))
                {
                    if (instruction.Values[RegisterShifter.Asr] is Integer)
                    {
                        var immediate = (uint)((Integer)instruction.Values[RegisterShifter.Asr]).Value;
                        if (immediate > 31 || immediate < 0)
                            throw new EncodingException(instruction.Values[RegisterShifter.Asr].Position.ToString("The shift must be between 0 and 31."));

                        binary |= immediate << index - 4;
                        binary |= 1u << 6; // Set the 6th bit.
                    }
                    else if (instruction.Values[RegisterShifter.Asr] is Register)
                    {
                        var rs = (Register)instruction.Values[RegisterShifter.Asr];
                        binary |= (uint)rs << index - 3;
                        binary |= 1u << 6; // Set the 6th bit.
                        binary |= 1u << 4; // Set the 4th bit.
                    }
                    else
                        throw new EncodingException(instruction.Mnemonic.Position.ToString("Expected a register or shift."));
                }
                else if (instruction.Values.ContainsKey(RegisterShifter.Ror))
                {
                    if (instruction.Values[RegisterShifter.Ror] is Integer)
                    {
                        var immediate = (uint)((Integer)instruction.Values[RegisterShifter.Ror]).Value;
                        if (immediate > 31 || immediate < 0)
                            throw new EncodingException(instruction.Values[RegisterShifter.Ror].Position.ToString("The shift must be between 0 and 31."));

                        binary |= immediate << index - 4;
                        binary |= 1u << 6; // Set the 6th bit.
                        binary |= 1u << 5; // Set the 5th bit.
                    }
                    else if (instruction.Values[RegisterShifter.Ror] is Register)
                    {
                        var rs = (Register)instruction.Values[RegisterShifter.Ror];
                        binary |= (uint)rs << index - 3;
                        binary |= 1u << 6; // Set the 6th bit.
                        binary |= 1u << 5; // Set the 5th bit.
                        binary |= 1u << 4; // Set the 4th bit.
                    }
                    else
                        throw new EncodingException(instruction.Mnemonic.Position.ToString("Expected a register or shift."));
                }
                else if (instruction.Values.ContainsKey(RegisterShifter.Rrx))
                {
                    binary |= 1u << 6; // Set the 6th bit.
                    binary |= 1u << 5; // Set the 5th bit.
                }

                binary |= rm << index - 11;
            }

            return index - 11;
        }

        /// <summary>
        /// Encodes an immediate integer data processing operand 
        /// as specified in section A5.1.3.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded immediate value.</returns>
        private static uint Encode(uint value)
        {
            var rotation = 0u;
            var immediate = value & 0xFFFFFFFFu;

            while (rotation < 16)
            {
                if (immediate < 256)
                    return (rotation << 8) | immediate;

                rotation++;
                immediate = ((immediate << 2) | (immediate >> 30)) & 0xFFFFFFFFu;
            }

            throw new Exception($"The immediate {value} could not be encoded.");
        }

        /// <summary>
        /// Converts an integer to binary.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>The binary value.</returns>
        public static string ToBinary(uint value)
        {
            var text = Convert.ToString(value, 2);
            text = text.PadLeft(32, '0');

            for (int i = 4; i <= text.Length; i += 4)
                text = text.Insert(i++, " ");

            return text.Substring(0, text.Length - 1);
        }
    }
}