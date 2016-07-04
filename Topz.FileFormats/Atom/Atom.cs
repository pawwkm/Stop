using System;
using System.Diagnostics.CodeAnalysis;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Defines an atom in an object file.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = Justifications.AcceptableConflict)]
    public abstract class Atom
    {
        private string name = "";

        /// <summary>
        /// The name of the atomic unit.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(value));

                name = value;
            }
        }

        /// <summary>
        /// If true this atom is some code or data otherwise; 
        /// a reference to an outside atom.
        /// </summary>
        public bool IsDefined
        {
            get;
            set;
        }

        /// <summary>
        /// If true outside atoms is allowed to reference this atom;
        /// otherwise only atoms in this atom's object file may reference it.
        /// </summary>
        public bool IsGlobal
        {
            get;
            set;
        }

        /// <summary>
        /// The size of the atom, in bytes.
        /// </summary>
        public abstract uint Size
        {
            get;
        }
    }
}