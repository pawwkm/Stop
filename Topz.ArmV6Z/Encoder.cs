using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Encodes various parts of an instruction.
    /// </summary>
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
        /// Encodes a shifter operand as specified on section A5.1.
        /// </summary>
        /// <param name="binary">The instruction to apply the operand to.</param>
        /// <param name="index">The index where the operand is applied to.</param>
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
        /// Encodes a load/store operand as specified on section A5.2.
        /// </summary>
        /// <param name="binary">The instruction to apply the operand to.</param>
        /// <param name="index">The index where the operand is applied to.</param>
        /// <param name="instruction">The instruction that carries the shifter operand.</param>
        /// <returns>The new index.</returns>
        public static int EncodeLoadStoreOperand(ref uint binary, int index, Instruction instruction)
        {
            if (instruction.Values.ContainsKey(Placeholders.Offset12))
            {
                // Set the 26th and 24th bit.
                binary |= 1 << 26;
                binary |= 1 << 24;

                var offset = instruction.Values[Placeholders.Offset12] as Integer;
                if (Math.Abs(offset.Value) > 4095)
                    throw new EncodingException(offset.Position.ToString("The offset exceeds 12 bits."));

                binary |= ((uint)Math.Abs(offset.Value) << index - 11);
                if (instruction.Values.ContainsKey(Symbols.ExclamationMark))
                    binary |= 1 << 21;
            }
            else if (instruction.Values.ContainsKey(Symbols.Plus + Placeholders.Rm) || instruction.Values.ContainsKey(Symbols.Minus + Placeholders.Rm))
            {
                // Set the 26th, 25th and 24th bit.
                binary |= 1 << 26;
                binary |= 1 << 25;
                binary |= 1 << 24;

                if (instruction.Values.ContainsKey(Symbols.ExclamationMark))
                    binary |= 1 << 21;

                var rm = 0u;
                if (instruction.Values.ContainsKey(Symbols.Plus + Placeholders.Rm))
                {
                    rm = (uint)((Register)instruction.Values[Symbols.Plus + Placeholders.Rm]);
                    binary |= 1 << 23;
                }
                else
                    rm = (uint)((Register)instruction.Values[Symbols.Minus + Placeholders.Rm]);

                binary |= rm << index - 11;
                if (instruction.Values.ContainsKey(Placeholders.Shift))
                {
                    var shift = 0u;

                    Integer immediate = null;
                    if (instruction.Values.ContainsKey(Placeholders.ShiftImmediate))
                        immediate = (Integer)instruction.Values[Placeholders.ShiftImmediate];

                    var register = instruction.Values[Placeholders.Shift] as RegisterShifter;
                    if (register == RegisterShifter.Lsl)
                    {
                        if (immediate.Value < 0 || immediate.Value > 31)
                            throw new EncodingException(immediate.Position.ToString("Immediate must be from 0 to 31."));
                    }
                    else if (register == RegisterShifter.Lsr)
                    {
                        shift = 1;
                        if (immediate.Value < 0 || immediate.Value > 32)
                            throw new EncodingException(immediate.Position.ToString("Immediate must be from 1 to 32."));
                    }
                    else if (register == RegisterShifter.Asr)
                    {
                        shift = 2;
                        if (immediate.Value < 0 || immediate.Value > 32)
                            throw new EncodingException(immediate.Position.ToString("Immediate must be from 1 to 32."));
                    }
                    else if (register == RegisterShifter.Ror)
                    {
                        shift = 3;
                        if (immediate.Value < 0 || immediate.Value > 31)
                            throw new EncodingException(immediate.Position.ToString("Immediate must be from 1 to 31."));
                    }
                    else if (register == RegisterShifter.Rrx)
                        shift = 3;

                    binary |= shift << 5;
                    if (immediate != null && immediate.Value != 32)
                        binary |= (uint)immediate.Value << 7;
                }
            }

            if (instruction.Values.ContainsKey(Placeholders.PostIndexed))
                binary &= ~(1u << 24);

            return index - 11;
        }

        /// <summary>
        /// Encodes a load/store operand as specified on section A5.3.
        /// </summary>
        /// <param name="binary">The instruction to apply the operand to.</param>
        /// <param name="index">The index where the operand is applied to.</param>
        /// <param name="instruction">The instruction that carries the shifter operand.</param>
        /// <returns>The new index.</returns>
        public static int EncodeMiscellaneousLoadStoreOperand(ref uint binary, int index, Instruction instruction)
        {
            if (instruction.Values.ContainsKey(Placeholders.Offset8))
            {
                // Set the 24th and 22th bit.
                binary |= 1 << 24;
                binary |= 1 << 22;

                var offset = instruction.Values[Placeholders.Offset8] as Integer;
                if (Math.Abs(offset.Value) > 4095)
                    throw new EncodingException(offset.Position.ToString("The offset exceeds 12 bits."));

                binary |= ((uint)Math.Abs(offset.Value) << index - 11);
                if (instruction.Values.ContainsKey(Symbols.ExclamationMark))
                    binary |= 1 << 21;
            }
            else if (instruction.Values.ContainsKey(Symbols.Plus + Placeholders.Rm) || instruction.Values.ContainsKey(Symbols.Minus + Placeholders.Rm))
            {
                // Set the 24th, 7th and 4th bit.
                binary |= 1 << 24;
                binary |= 1 << 7;
                binary |= 1 << 4;

                if (instruction.Values.ContainsKey(Symbols.ExclamationMark))
                    binary |= 1 << 21;

                var rm = 0u;
                if (instruction.Values.ContainsKey(Symbols.Plus + Placeholders.Rm))
                {
                    rm = (uint)((Register)instruction.Values[Symbols.Plus + Placeholders.Rm]);
                    binary |= 1 << 23;
                }
                else
                    rm = (uint)((Register)instruction.Values[Symbols.Minus + Placeholders.Rm]);

                binary |= rm << index - 3;
            }

            if (instruction.Values.ContainsKey(Placeholders.PostIndexed))
                binary &= ~(1u << 24);

            return index - 4;
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
    }
}