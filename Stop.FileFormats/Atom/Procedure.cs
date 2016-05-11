using System.Collections.Generic;

namespace Stop.FileFormats.Atom
{
    /// <summary>
    /// Defines a named chunk of executable code.
    /// </summary>
    public sealed class Procedure : Atom
    {
        private List<byte> code = new List<byte>();

        private List<Reference> references = new List<Reference>();

        /// <summary>
        /// The other atoms referred to by this procedure.
        /// </summary>
        public IList<Reference> References
        {
            get
            {
                return references;
            }
        }

        /// <summary>
        /// The actual code of the procedure.
        /// </summary>
        public IList<byte> Code
        {
            get
            {
                return code;
            }
        }

        /// <summary>
        /// The size of the atom, in bytes.
        /// </summary>
        public override uint Size
        {
            get
            {
                return (uint)code.Count;
            }
        }

        /// <summary>
        /// If true the procedure is the starting point of the program.
        /// </summary>
        public bool IsMain
        {
            get;
            set;
        }
    }
}