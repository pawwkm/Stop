using Pote;
using System;

namespace Stop.FileFormats
{
    /// <summary>
    /// Defines an atom in an object file.
    /// </summary>
    public abstract class Atom
    {
        private string name = "";

        private byte sizeOfAddress = 4;

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
        /// If true this atom can't be relocated to another address
        /// otherwise; the atom may be relocated.
        /// </summary>
        public bool IsAddressFixed
        {
            get;
            set;
        }

        /// <summary>
        /// The size of an address in bytes.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not 2, 4 or 8.
        /// </exception>
        public byte SizeOfAddress
        {
            get
            {
                return sizeOfAddress;
            }
            set
            {
                if (!sizeOfAddress.IsOneOf(2, 4, 8))
                    throw new ArgumentException("The address size can only be 2, 4 or 8 bytes long", nameof(value));

                sizeOfAddress = value;
            }
        }

        /// <summary>
        /// The absolute address which it is assumed the atom begins at 
        /// when it is loaded into memory.
        /// </summary>
        public ulong Address
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