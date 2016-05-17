using Pote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stop.FileFormats.Atom
{
    /// <summary>
    /// Reads Atom object files.
    /// </summary>
    public class AtomReader
    {
        private List<Tuple<Procedure, uint, bool, byte, uint>> references = new List<Tuple<Procedure, uint, bool, byte, uint>>();

        private BinaryReader reader;

        /// <summary>
        /// True if there are no bytes left in the <see cref="reader"/>.
        /// </summary>
        private bool EndOfFile
        {
            get
            {
                return reader.BaseStream.IsEndOfStream();
            }
        }

        /// <summary>
        /// The current position in the object file.
        /// </summary>
        private long Position
        {
            get
            {
                return reader.BaseStream.Position;
            }
        }

        /// <summary>
        /// The number of bytes left in <see cref="reader"/>.
        /// </summary>
        private long BytesLeft
        {
            get
            {
                return reader.BaseStream.BytesLeft();
            }
        }

        /// <summary>
        /// Reads the atoms from the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source to read atoms from.</param>
        /// <returns>The atoms read from the <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// There is no header on the current position in the <paramref name="source"/>.
        /// </exception>
        /// <exception cref="InvalidObjectFileException">
        /// <paramref name="source"/> is an invalid object file.
        /// </exception>
        public ObjectFile Read(Stream source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Length - source.Position <= 13)
                throw new ArgumentException("There is no header on the current position.", nameof(source));

            var file = new ObjectFile();
            using (reader = new BinaryReader(source))
            {
                // This number is ascii for "atom" backwards.
                if (reader.ReadInt32() != 0x6D6F7461)
                    throw new InvalidObjectFileException("This is not an atom object file.");
                if (reader.ReadUInt16() != 1)
                    throw new InvalidObjectFileException("This object file is not using version one.");

                file.IsOriginSet = reader.ReadBoolean();
                file.Origin = reader.ReadUInt64();

                while (!EndOfFile)
                {
                    switch (reader.ReadByte())
                    {
                        case 0:
                            file.Atoms.Add(Procedure());
                            break;
                        case 1:
                            file.Atoms.Add(NullTerminatedString());
                            break;
                        case 2:
                            file.Atoms.Add(Data());
                            break;
                        default:
                            throw new InvalidObjectFileException("Invalid atom type at " + ToHex(reader.BaseStream.Position - 1));
                    }
                }
            }

            ResolveReferences(file);

            return file;
        }

        /// <summary>
        /// Returns the hex representation of the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The number to represent in hex.</param>
        /// <returns>The hex representation of the given <paramref name="number"/>.</returns>
        private static string ToHex(long number)
        {
            if (number <= byte.MaxValue)
                return "0x" + number.ToString("X2");

            if (number <= ushort.MaxValue)
                return "0x" + number.ToString("X4");

            if (number <= uint.MaxValue)
                return "0x" + number.ToString("X8");

            return "0x" + number.ToString("X16");
        }

        /// <summary>
        /// Reads a procedure from the object file.
        /// </summary>
        /// <returns>The read procedure.</returns>
        private Procedure Procedure()
        {
            var procedure = new Procedure();
            Atom(procedure);

            procedure.IsMain = IsMain();
            foreach (var b in Code())
                procedure.Code.Add(b);

            References(procedure);

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
        /// Checks if the address of the current atom is in little endian.
        /// </summary>
        /// <returns>True if the address of the current atom is in little endian; otherwise false.</returns>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private bool IsAddressInLittleEndian()
        {
            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected 'is little endian' bool.");

            return reader.ReadBoolean();
        }

        /// <summary>
        /// Reads an address of a given size. 
        /// </summary>
        /// <param name="size">The size of the address to read.</param>
        /// <param name="isLittleEndian">True if the address is expected to be encoded in little endian.</param>
        /// <returns>The read address.</returns>
        /// <exception cref="InvalidOperationException">
        /// Unexpected end of object file. 
        /// </exception>
        private ulong Address(byte size, bool isLittleEndian)
        {
            if (size == 2)
            {
                if (BytesLeft < 2)
                    throw new InvalidObjectFileException("Unexpected end of object file. Expected a 16 bit address.");

                if (isLittleEndian)
                    return reader.ReadUInt16();

                return reader.ReadBigEndianUInt16();
            }
            if (size == 4)
            {
                if (BytesLeft < 4)
                    throw new InvalidObjectFileException("Unexpected end of object file. Expected a 32 bit address.");

                if (isLittleEndian)
                    return reader.ReadUInt32();

                return reader.ReadBigEndianUInt32();
            }

            if (BytesLeft < 8)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a 64 bit address.");

            if (isLittleEndian)
                return reader.ReadUInt64();

            return reader.ReadBigEndianUInt64();
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
            while (!EndOfFile)
            {
                var b = reader.ReadByte();
                switch (b)
                {
                    case 0:
                        break;
                    default:
                        bytes.Add(b);
                        break;
                }

                if (b == 0)
                    break;
            }

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
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a code block of " + ToHex(bytes.Length) + " bytes but " + ToHex(amount) + " was read.");

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
            var s = new NullTerminatedString();
            Atom(s);

            if (EndOfFile)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected null terminated string.");

            var bytes = new List<byte>();
            while (!EndOfFile)
            {
                var b = reader.ReadByte();
                switch (b)
                {
                    case 0:
                        break;
                    default:
                        bytes.Add(b);
                        break;
                }

                if (b == 0)
                    break;
            }

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
            var data = new Data();
            Atom(data);

            if (BytesLeft < 4)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a the size of a data block.");

            var size = reader.ReadUInt32();

            var bytes = new byte[size];
            var amount = reader.Read(bytes, 0, bytes.Length);

            if (amount != bytes.Length)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected a data block of " + ToHex(bytes.Length) + " bytes but " + ToHex(amount) + " was read.");

            foreach (var b in bytes)
                data.Content.Add(b);

            return data;
        }

        /// <summary>
        /// Reads the references of a procedure.
        /// </summary>
        /// <param name="owner">The owner of references ahead.</param>
        /// <exception cref="InvalidObjectFileException">
        /// Unexpected end of object file.
        /// </exception>
        private void References(Procedure owner)
        {
            if (BytesLeft < 2)
                throw new InvalidObjectFileException("Unexpected end of object file. Expected the 16 bit number for amount of references.");

            var amount = reader.ReadUInt16();
            for (int i = 0; i < amount; i++)
            {
                if (BytesLeft < 4)
                    throw new InvalidObjectFileException("Unexpected end of object file. Expected the number of an atom.");

                var number = reader.ReadUInt32();
                var isLittleEndian = IsAddressInLittleEndian();

                if (EndOfFile)
                    throw new InvalidObjectFileException("Unexpected end of object file. Expected 'address size' byte.");

                var size = reader.ReadByte();
                if (!size.IsOneOf(2, 4))
                    throw new InvalidObjectFileException("The address size at " + ToHex(Position - 1) + " is invalid. It must be 2 or 4.");

                var address = (uint)Address(size, isLittleEndian);

                references.Add(new Tuple<Procedure, uint, bool, byte, uint>(owner, number, isLittleEndian, size, address));
            }
        }

        /// <summary>
        /// Resolves the references the given <paramref name="file"/> have.
        /// </summary>
        /// <param name="file">The atoms with references.</param>
        /// <exception cref="InvalidObjectFileException">
        /// There are references with overlapping addresses.
        /// </exception>
        private void ResolveReferences(ObjectFile file)
        {
            foreach (var tuple in references)
            {
                if (file.Atoms.Count < tuple.Item2)
                {
                    var message = string.Format("The atom called '{0}' has a reference to atom number {1} which doesn't exist.", tuple.Item1.Name, tuple.Item2);

                    throw new InvalidObjectFileException(message);
                }

                var reference = new Reference(file.Atoms[(int)tuple.Item2]);
                reference.IsAddressInLittleEndian = tuple.Item3;
                reference.SizeOfAddress = tuple.Item4;
                reference.Address = tuple.Item5;

                foreach (var r in tuple.Item1.References)
                {
                    if (r.IsOverlapping(reference))
                    {
                        var message = string.Format("{0}'s reference to '{1}' has an overlapping address with the reference to '{2}' at {3}.", tuple.Item1.Name, r.Atom.Name, reference.Atom.Name, ToHex(r.Address));

                        throw new InvalidObjectFileException(message);
                    }
                }

                tuple.Item1.References.Add(reference);
            }
        }
    }
}