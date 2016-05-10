using Pote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stop.FileFormats
{
    /// <summary>
    /// Reads Atom object files.
    /// </summary>
    public class AtomReader
    {
        private BinaryReader reader;

        /// <summary>
        /// True if there are no bytes left in the <see cref="reader"/>.
        /// </summary>
        private bool EndOfFile
        {
            get
            {
                return reader.BaseStream.Position == reader.BaseStream.Length;
            }
        }

        /// <summary>
        /// The current position in the object file as hexidecimal.
        /// </summary>
        private string HexPosition
        {
            get
            {
                return reader.BaseStream.Position.ToString("X");
            }
        }

        /// <summary>
        /// The number of bytes left in <see cref="reader"/>.
        /// </summary>
        private long BytesLeft
        {
            get
            {
                return reader.BaseStream.Length - reader.BaseStream.Position;
            }
        }

        /// <summary>
        /// Reads the atoms from the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source to read atoms from.</param>
        /// <returns>The atoms read from the <paramref name="source"/>.</returns>
        public IList<Atom> Read(Stream source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Length - source.Position < 6)
                throw new ArgumentException("There is no header on the current position.", nameof(source));

            var atoms = new List<Atom>();
            using (reader = new BinaryReader(source))
            {
                // This number is ascii for "atom" backwards.
                if (reader.ReadInt32() != 1836020833)
                    throw new InvalidObjectFileException("This is not an atom object file.");
                if (reader.ReadUInt16() != 1)
                    throw new InvalidObjectFileException("This object file is not using version one.");

                while (!EndOfFile)
                {
                    switch (reader.PeekChar())
                    {
                        case 0:
                            atoms.Add(Procedure());
                            break;
                        case 1:
                            atoms.Add(NullTerminatedString());
                            break;
                        case 2:
                            atoms.Add(Data());
                            break;
                        default:
                            throw new InvalidObjectFileException("Invalid atom type at " + HexPosition);
                    }
                }
            }

            return atoms;
        }

        /// <summary>
        /// Reads a procedure from the object file.
        /// </summary>
        /// <returns>The read procedure.</returns>
        private Procedure Procedure()
        {
            reader.ReadByte(); // Skip the atom type.

            var procedure = new Procedure();
            Atom(procedure);

            procedure.IsMain = IsMain();
            foreach (var b in Code())
                procedure.Code.Add(b);

            foreach (var r in References())
                procedure.References.Add(r);

            return procedure;
        }

        /// <summary>
        /// Reads the base data of an atom.
        /// </summary>
        /// <param name="atom">The atom to populate.</param>
        /// <exception cref="InvalidObjectFileException">
        /// The object file is invalid in some way.
        /// </exception>
        private void Atom(Atom atom)
        {
            atom.IsDefined = IsDefined();
            atom.IsGlobal = IsGlobal();
            atom.IsAddressFixed = IsAddressFixed();
            atom.SizeOfAddress = SizeOfAddress();
            atom.Address = Address(atom.SizeOfAddress);
            atom.Name = Name();
        }

        /// <summary>
        /// Checks if the current atom is defined in the object file.
        /// </summary>
        /// <returns>True if the current atom is defined in the object file; otherwise false.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private bool IsDefined()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected 'is defined' bool.");

            return reader.ReadBoolean();
        }

        /// <summary>
        /// Checks if the current atom is globally available.
        /// </summary>
        /// <returns>True if the current atom is globally available; otherwise false.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private bool IsGlobal()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected 'is global' bool.");

            return reader.ReadBoolean();
        }

        /// <summary>
        /// Checks if the address of the current atom is fixed.
        /// </summary>
        /// <returns>True if the address of the current atom is fixed; otherwise false.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private bool IsAddressFixed()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected 'is address fixed' bool.");

            return reader.ReadBoolean();
        }

        /// <summary>
        /// Checks if the address of the current atom is fixed.
        /// </summary>
        /// <returns>True if the address of the current atom is fixed; otherwise false.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file. The given size is not 2, 4, or 8.
        /// </exception>
        private byte SizeOfAddress()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected 'address size' byte.");

            var size = reader.ReadByte();
            if (!size.IsOneOf(2, 4, 8))
                throw new InvalidObjectFileException("The address size at " + HexPosition + " is invalid. It must be 2, 4 or 8.");

            return size;
        }

        /// <summary>
        /// Reads an address of a given size. 
        /// </summary>
        /// <param name="size">The size of the address to read.</param>
        /// <returns>The read address.</returns>
        /// <exception cref="InvalidOperationException">
        /// Unexpected end of object file. 
        /// </exception>
        private ulong Address(byte size)
        {
            if (size == 2)
            {
                if (BytesLeft < 2)
                    throw new InvalidObjectFileException("Unexpected end of object file. Expected a 16 bit address.");

                return reader.ReadUInt16();
            }
            if (size == 4)
            {
                if (BytesLeft < 4)
                    throw new InvalidObjectFileException("Unexpected end of object file. Expected a 32 bit address.");

                return reader.ReadUInt32();
            }

            if (BytesLeft < 8)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a 64 bit address.");

            return reader.ReadUInt64();
        }

        /// <summary>
        /// Reads the name of an atom.
        /// </summary>
        /// <returns>The name of the atom.</returns>
        /// <exception cref="InvalidOperationException">
        /// Unexpected end of object file. 
        /// </exception>
        private string Name()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected the name of an atom.");

            var bytes = new List<byte>();
            while (!reader.PeekChar().IsOneOf(-1, 0))
                bytes.Add(reader.ReadByte());

            reader.ReadByte(); // Skip null terminater.

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Checks if the current procedure is the main procedure.
        /// </summary>
        /// <returns>True if the current procedure is the main procedure.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private bool IsMain()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected 'is main' bool.");

            return reader.ReadBoolean();
        }

        /// <summary>
        /// Reads a block of code.
        /// </summary>
        /// <returns>The read code block.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private IList<byte> Code()
        {
            if (BytesLeft < 4)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected the 32 bit number for code size.");

            var size = reader.ReadUInt32();

            var bytes = new byte[size];
            var amount = reader.Read(bytes, 0, bytes.Length);

            if (amount != bytes.Length)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a code block of " + bytes.Length + " but " + amount + " was read.");

            return bytes;
        }

        /// <summary>
        /// Reads a null terminated string from the object file.
        /// </summary>
        /// <returns>The read string.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private NullTerminatedString NullTerminatedString()
        {
            reader.ReadByte(); // Skip the atom type.

            var s = new NullTerminatedString();
            Atom(s);

            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected null terminated string.");

            var bytes = new List<byte>();
            while (!reader.PeekChar().IsOneOf(-1, 0))
                bytes.Add(reader.ReadByte());

            reader.ReadByte(); // Skip null terminater.
            s.Content = Encoding.UTF8.GetString(bytes.ToArray());

            return s;
        }

        /// <summary>
        /// Reads a data block from the object file.
        /// </summary>
        /// <returns>The read data block.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private Data Data()
        {
            reader.ReadByte(); // Skip the atom type.

            var data = new Data();
            Atom(data);

            if (BytesLeft < 4)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a the size of a data block.");

            var size = reader.ReadUInt32();

            var bytes = new byte[size];
            var amount = reader.Read(bytes, 0, bytes.Length);

            if (amount != bytes.Length)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a data block of " + bytes.Length + " but " + amount + " was read.");

            foreach (var b in bytes)
                data.Content.Add(b);

            return data;
        }

        /// <summary>
        /// Reads the references of a procedure.
        /// </summary>
        /// <returns>The references of a procedure.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private IList<Reference> References()
        {
            var amount = reader.ReadUInt16();

            return new Reference[0];
        }
    }
}