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
            Atom = referenced ?? throw new ArgumentNullException(nameof(referenced));
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
                atom = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}