using Pote.Text;
using System;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// A single procedure in a program.
    /// </summary>
    internal sealed class Procedure : Node, INamedNode
    {
        private List<Instruction> instructions = new List<Instruction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Procedure"/> class.
        /// </summary>
        /// <param name="position">The position of the node in the program's source code.</param>
        /// <param name="name">Name of the procedure.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Procedure(InputPosition position, string name) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        /// <summary>
        /// Name of the procedure.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the this is the main procedure of the program.
        /// </summary>
        public bool IsMain
        {
            get;
            set;
        }

        /// <summary>
        /// The instruction that the procedure is composed of in 
        /// the order they are to be executed.
        /// </summary>
        public IList<Instruction> Instructions
        {
            get
            {
                return instructions;
            }
        }
    }
}