using System.Collections;
using System.Collections.Generic;

namespace Stop.FileFormats.Atom
{
    /// <summary>
    /// An Atom object file.
    /// </summary>
    public sealed class ObjectFile : IEnumerable<Atom>
    {
        private List<Atom> atoms = new List<Atom>();

        /// <summary>
        /// The atoms of the object file.
        /// </summary>
        public IList<Atom> Atoms
        {
            get
            {
                return atoms;
            }
        }

        /// <summary>
        /// The offset from zero which the <see cref="Atoms"/> have been assembled.
        /// </summary>
        public ulong Origin
        {
            get;
            set;
        }

        /// <summary>
        /// True if the <see cref="Origin"/> must be respected.
        /// </summary>
        public bool IsOriginSet
        {
            get;
            set;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the atoms.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the atoms.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Atoms.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the atoms.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the atoms.</returns>
        IEnumerator<Atom> IEnumerable<Atom>.GetEnumerator()
        {
            return Atoms.GetEnumerator();
        }
    }
}