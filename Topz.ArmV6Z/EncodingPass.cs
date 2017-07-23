using Pote;
using System;
using System.Linq;
using Topz.FileFormats.Atom;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Encodes a program into object code.
    /// </summary>
    internal sealed class EncodingPass : Pass
    {
        private FileFormats.Atom.Procedure current;

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
        public override void Visit(Program program)
        {
            Code = new ObjectFile();
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
            current = new FileFormats.Atom.Procedure();
            current.Name = procedure.Name;
            current.IsDefined = !procedure.IsExternal;
            current.IsMain = procedure.IsMain;

            Code.Atoms.Add(current);
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
            int index = 31;

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
                else if (chunk == "shifter_operand")
                    index = Encoder.EncodeShifterOperand(ref value, index, instruction);
                else if (chunk == "signed_immed_24")
                {
                    index -= 24;
                }
                else if (chunk == "I")
                    index = Encoder.Encode(ref value, index, instruction.Values.ContainsKey(Placeholders.Immediate));
                else
                    throw new NotSupportedException($"The encoding of '{chunk}' is not supported.");
            }

            foreach (var b in BitConverter.GetBytes(value).Reverse())
                current.Code.Add(b);
        }
    }
}