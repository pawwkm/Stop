using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Reference to a global address outside of a <see cref="Procedure"/>.
    /// </summary>
    public sealed class GlobalReference : Reference
    {
        private Atom atom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        /// <param name="referenced">The atom that is being referenced.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="referenced"/> is null.
        /// </exception>
        public GlobalReference(Atom referenced)
        {
            if (referenced == null)
                throw new ArgumentNullException(nameof(referenced));

            Atom = referenced;
        }

        /// <summary>
        /// The atom that is being referenced.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public Atom Atom
        {
            get
            {
                return atom;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                atom = value;
            }
        }

        /// <summary>
        /// Checks if the <paramref name="reference"/>'s address overlaps with 
        /// the address with this reference.
        /// </summary>
        /// <param name="reference">The reference the check.</param>
        /// <returns>True if the two references have overlapping addresses; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reference"/> is null.
        /// </exception>
        public bool IsOverlapping(Reference reference)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            return false;
        }
    }
}