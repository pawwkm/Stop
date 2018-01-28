using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines a pass in the assembler.
    /// </summary>
    internal abstract class Pass
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
        protected virtual void Visit(Procedure procedure)
        {
            if (procedure == null)
                throw new ArgumentNullException(nameof(procedure));

            foreach (dynamic instruction in procedure.Instructions)
                Visit(instruction);
        }

        /// <summary>
        /// Visits an instruction node.
        /// </summary>
        /// <param name="instruction">The instruction node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        protected virtual void Visit(Instruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));
        }

        /// <summary>
        /// Visits a data node.
        /// </summary>
        /// <param name="data">The data node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> is null.
        /// </exception>
        protected virtual void Visit(Data data)
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
        protected virtual void Visit(String s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
        }
    }
}