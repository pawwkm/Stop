using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines a pass in the assembler.
    /// </summary>
    internal interface IPass
    {
        /// <summary>
        /// Visits a program node.
        /// </summary>
        /// <param name="program">The program node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="program"/> is null.
        /// </exception>
        void Visit(Program program);

        /// <summary>
        /// Visits a procedure node.
        /// </summary>
        /// <param name="procedure">The procedure node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="procedure"/> is null.
        /// </exception>
        void Visit(Procedure procedure);

        /// <summary>
        /// Visits an instruction node.
        /// </summary>
        /// <param name="instruction">The instruction node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instruction"/> is null.
        /// </exception>
        void Visit(Instruction instruction);

        /// <summary>
        /// Visits a data node.
        /// </summary>
        /// <param name="data">The data node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> is null.
        /// </exception>
        void Visit(Data data);

        /// <summary>
        /// Visits a string node.
        /// </summary>
        /// <param name="s">The string node to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is null.
        /// </exception>
        void Visit(String s);
    }
}