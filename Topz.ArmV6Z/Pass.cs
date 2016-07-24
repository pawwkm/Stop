using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines a pass in the assembler.
    /// </summary>
    internal abstract class Pass : IPass
    {
        /// <summary>
        /// Visits a program node then visits all the procedures, strings and data nodes.
        /// </summary>
        /// <param name="program">The program node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="program"/> is null.
        /// </exception>
        public virtual void Visit(Program program)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            foreach (Procedure procedure in program.Procedures)
                Visit(procedure);

            foreach (String s in program.Strings)
                Visit(s);

            foreach (Data data in program.Data)
                Visit(data);
        }

        /// <summary>
        /// Visits a procedure node.
        /// </summary>
        /// <param name="procedure">The procedure node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="procedure"/> is null.
        /// </exception>
        public virtual void Visit(Procedure procedure)
        {
            if (procedure == null)
                throw new ArgumentNullException(nameof(procedure));

            foreach (dynamic instruction in procedure.Instructions)
                Visit(instruction);
        }

        /// <summary>
        /// Visits a data node.
        /// </summary>
        /// <param name="data">The data node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> is null.
        /// </exception>
        public virtual void Visit(Data data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Visits a string node.
        /// </summary>
        /// <param name="s">The string node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is null.
        /// </exception>
        public virtual void Visit(String s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(AddWithCarryInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(AddInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(AndInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(BranchInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(BitClearInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(BreakPointInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an instruction.
        /// </summary>
        /// <param name="instruction">The instruction to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        public virtual void Visit(BranchAndExchangeInstruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits an un supported node type.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The type of <paramref name="node"/> is not supported by this pass.
        /// </exception>
        public virtual void Visit(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            string name = node.GetType().FullName;

            throw new InvalidOperationException("The node type '" + name + "' is not supported by this pass.");
        }
    }
}