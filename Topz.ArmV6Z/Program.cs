using Pote.Text;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Represents a single program.
    /// </summary>
    internal sealed class Program : Node
    {
        private List<Procedure> procedure = new List<Procedure>();

        private List<String> strings = new List<String>();

        private List<Data> data = new List<Data>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program() : base(new InputPosition())
        {
        }

        /// <summary>
        /// All the procedures in the program.
        /// </summary>
        public IList<Procedure> Procedures
        {
            get
            {
                return procedure;
            }
        }

        /// <summary>
        /// All the strings in the program.
        /// </summary>
        public IList<String> Strings
        {
            get
            {
                return strings;
            }
        }

        /// <summary>
        /// All the data in the program.
        /// </summary>
        public IList<Data> Data
        {
            get
            {
                return data;
            }
        }
    }
}