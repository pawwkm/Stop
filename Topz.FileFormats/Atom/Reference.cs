using System;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Defines a reference from a <see cref="Procedure"/> to an <see cref="Atom"/>.
    /// </summary>
    public sealed class Reference
    {
        private Atom atom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        /// <param name="referenced">The atom that is being referenced.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="referenced"/> is null.
        /// </exception>
        public Reference(Atom referenced)
        {
            if (referenced == null)
                throw new ArgumentNullException(nameof(referenced));

            Atom = referenced;
        }

        /// <summary>
        /// The type of address to relocate.
        /// </summary>
        public AddressType AddressType
        {
            get;
            set;
        }

        /// <summary>
        /// This is the address within the code block of the procedure to
        /// relocate to match the address of the referenced atom.
        /// </summary>
        public uint Address
        {
            get;
            set;
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