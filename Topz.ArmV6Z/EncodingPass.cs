using Pote;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Topz.FileFormats.Atom;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Encodes a program into object code.
    /// </summary>
    internal sealed class EncodingPass : Pass
    {
        private FileFormats.Atom.Procedure current;

        private Procedure fuck;

        /// <summary>
        /// The assembled object code.
        /// </summary>
        public ObjectFile Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Visits a program node then visits all the procedures, strings and data nodes.
        /// </summary>
        /// <param name="program">The program node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="program"/> is null.
        /// </exception>
        /// <exception cref="EncodingException">
        /// A problem occurred when encoding.
        /// </exception>
        public override void Visit(Program program)
        {
            Code = new ObjectFile();

            // Add all the atoms to allow future references.
            foreach (var p in program.Procedures)
                Code.Atoms.Add(new FileFormats.Atom.Procedure() { Name = p.Name });

            foreach (var s in program.Strings)
                Code.Atoms.Add(new NullTerminatedString() { Name = s.Name });

            foreach (var d in program.Data)
                Code.Atoms.Add(new FileFormats.Atom.Data() { Name = d.Name });

            base.Visit(program);
        }

        /// <summary>
        /// Visits a procedure node.
        /// </summary>
        /// <param name="procedure">The procedure node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="procedure"/> is null.
        /// </exception>
        protected override void Visit(Procedure procedure)
        {
            fuck = procedure;
            current = Code.Atoms.OfType<FileFormats.Atom.Procedure>().First(x => x.Name == procedure.Name);
            current.Name = procedure.Name;
            current.IsDefined = !procedure.IsExternal;
            current.IsMain = procedure.IsMain;

            base.Visit(procedure);
        }

        /// <summary>
        /// Visits an instruction node.
        /// </summary>
        /// <param name="instruction">The instruction node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        protected override void Visit(Instruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));

            var value = 0u;
            var index = 31;
            
            var skipedFirstAddressMode = false;
            foreach (var chunk in instruction.Encoding.Split(' '))
            {
                if (chunk == "cond")
                    index = Encoder.Encode(ref value, index, instruction.Mnemonic.Condition);
                else if (chunk.IsBinary())
                    index = Encoder.Encode(ref value, index, chunk);
                else if (chunk.IsOneOf("Rn", "Rm", "Rd"))
                    index = Encoder.Encode(ref value, index, (Register)instruction.Values[$"<{chunk}>"]);
                else if (chunk == "S")
                    index = Encoder.Encode(ref value, index, instruction.Mnemonic.Bit.HasFlag(Bit.S));
                else if (chunk == "L")
                    index = Encoder.Encode(ref value, index, instruction.Mnemonic.Bit.HasFlag(Bit.L));
                else if (chunk == "I")
                    index = Encoder.Encode(ref value, index, instruction.Values.ContainsKey(Placeholders.Immediate));
                else if (chunk == "U")
                {
                    if (instruction.Values.ContainsKey(Placeholders.Offset12))
                        index = Encoder.Encode(ref value, index, ((Integer)instruction.Values[Placeholders.Offset12]).Value >= 0);
                    else if (instruction.Values.ContainsKey(Placeholders.Offset8))
                        index = Encoder.Encode(ref value, index, ((Integer)instruction.Values[Placeholders.Offset8]).Value >= 0);
                    else if (instruction.Values.ContainsKey(Placeholders.ShiftImmediate))
                        index = Encoder.Encode(ref value, index, ((Integer)instruction.Values[Placeholders.ShiftImmediate]).Value >= 0);
                    else if (instruction.Values.ContainsKey(Symbols.Plus + Placeholders.Rm))
                        index = Encoder.Encode(ref value, index, true);
                    else if (instruction.Values.ContainsKey(Symbols.Minus + Placeholders.Rm))
                        index = Encoder.Encode(ref value, index, false);
                    else
                        throw new EncodingException(instruction.Mnemonic.Position.ToString("Could not encode the U bit."));
                }
                else if (chunk == "P")
                    index = Encoder.Encode(ref value, index, IsPreIndexed(instruction));
                else if (chunk == "W")
                    index = Encoder.Encode(ref value, index, false);
                else if (chunk == "shifter_operand")
                    index = Encoder.EncodeShifterOperand(ref value, index, instruction);
                else if (chunk == "addr_mode")
                {
                    if (Regex.Matches(instruction.Encoding, "addr_mode").Count == 2)
                    {
                        if (!skipedFirstAddressMode)
                        {
                            index -= 4;
                            skipedFirstAddressMode = true;
                        }
                        else
                            index = Encoder.EncodeMiscellaneousLoadStoreOperand(ref value, index, instruction);                        
                    }
                    else
                        index = Encoder.EncodeLoadStoreOperand(ref value, index, instruction);
                }
                else if (chunk == "signed_immed_24")
                {
                    index -= 24;
                    if (instruction.Values[Placeholders.TargetAddress] is Integer)
                    {
                        var address = new AbsoluteAddress((ulong)((Integer)instruction.Values[Placeholders.TargetAddress]).Value);
                        Code.Atoms.Add(address);

                        current.References.Add(new GlobalReference(address)
                        {
                            AddressType = AddressType.ArmTargetAddress,
                            Address = (uint)(current.Code.Count / 4)
                        });
                    }
                    else
                    {
                        var label = (Identifier)instruction.Values[Placeholders.TargetAddress];
                        var local = fuck.Instructions.FirstOrDefault(x => x.Label.Name == label.Name);

                        if (label != null)
                        {
                            current.References.Add(new LocalReference()
                            {
                                AddressType = AddressType.ArmTargetAddress,
                                Address = (uint)(current.Code.Count / 4),
                                Target = local.Label.Address
                            });
                        }
                        else
                        {
                            var global = Code.Atoms.FirstOrDefault(x => x.Name == label.Name);
                            if (global == null)
                                throw new EncodingException(label.Position.ToString($"The label '{label.Name}' is undefined."));

                            current.References.Add(new GlobalReference(global)
                            {
                                AddressType = AddressType.ArmTargetAddress,
                                Address = (uint)(current.Code.Count / 4)
                            });
                        }
                    }
                }
                else
                    throw new NotSupportedException($"The encoding of '{chunk}' is not supported.");
            }

            foreach (var b in BitConverter.GetBytes(value).Reverse())
                current.Code.Add(b);
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

        /// <summary>
        /// Checks if a instruction uses a pre-indexed operand.
        /// </summary>
        /// <param name="instruction">The instruction to check.</param>
        /// <returns>True if the instruction's operand is pre-indexed.</returns>
        private static bool IsPreIndexed(Instruction instruction)
        {
            if (instruction.Values.ContainsKey(Placeholders.Offset12))
                return true;
            if (instruction.Values.ContainsKey(Symbols.Plus + Placeholders.Rm))
                return true;
            if (instruction.Values.ContainsKey(Symbols.Minus + Placeholders.Rm))
                return true;
            if (instruction.Values.ContainsKey(Placeholders.Shift))
                return true;

            return instruction.Values.ContainsKey(Symbols.ExclamationMark);
        }
    }
}