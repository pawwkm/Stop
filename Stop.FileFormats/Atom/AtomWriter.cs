using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stop.FileFormats.Atom
{
    /// <summary>
    /// Writes Atom object files.
    /// </summary>
    public class AtomWriter
    {
        private BinaryWriter writer;

        private IList<Atom> list;

        /// <summary>
        /// Writes an object file to a stream.
        /// </summary>
        /// <param name="atoms">The atoms of the object file.</param>
        /// <param name="destination">The stream the object file is written to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="atoms"/> or <paramref name="destination"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="destination"/> is read only.
        /// </exception>
        public void Write(IList<Atom> atoms, Stream destination)
        {
            if (atoms == null)
                throw new ArgumentNullException(nameof(atoms));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("The stream is read only.", nameof(destination));

            using (writer = new BinaryWriter(destination, Encoding.UTF8, true))
            {
                // Write header.
                writer.Write(0x6D6F7461);   // Ascii for "atom" backwards.
                writer.Write((ushort)1);

                list = atoms;
                foreach (dynamic atom in atoms)
                    Write(atom);
            }
        }

        /// <summary>
        /// Writes a procedure.
        /// </summary>
        /// <param name="procedure">The procedure to write.</param>
        private void Write(Procedure procedure)
        {
            Write(procedure as Atom, 0x00);

            writer.Write(procedure.IsMain);
            writer.Write(procedure.Size);
            writer.Write(procedure.Code.ToArray(), 0, procedure.Code.Count);
            writer.Write((ushort)procedure.References.Count);

            foreach (var reference in procedure.References)
            {
                writer.Write((uint)list.IndexOf(reference.Atom));
                writer.Write(reference.IsAddressInLittleEndian);
                writer.Write(reference.SizeOfAddress);

                Write(reference.Address, reference.SizeOfAddress, reference.IsAddressInLittleEndian);
            }
        }

        /// <summary>
        /// Writes a string atom.
        /// </summary>
        /// <param name="s">The string to write.</param>
        private void Write(NullTerminatedString s)
        {
            Write(s as Atom, 0x01);

            var bytes = Encoding.UTF8.GetBytes(s.Content);
            writer.Write(bytes, 0, bytes.Length);
            writer.Write((byte)0x00);
        }

        /// <summary>
        /// Writes a data atom.
        /// </summary>
        /// <param name="data">The data to write.</param>
        private void Write(Data data)
        {
            Write(data as Atom, 0x02);

            writer.Write(data.Size);
            writer.Write(data.Content.ToArray(), 0, data.Content.Count);
        }

        /// <summary>
        /// Writes the base part of an atom.
        /// </summary>
        /// <param name="atom">The atom to write.</param>
        /// <param name="type">The type of <paramref name="atom"/>.</param>
        private void Write(Atom atom, byte type)
        {
            writer.Write(type);
            writer.Write(atom.IsDefined);
            writer.Write(atom.IsGlobal);
            writer.Write(atom.IsAddressFixed);
            writer.Write(atom.IsAddressInLittleEndian);

            writer.Write(atom.SizeOfAddress);
            Write(atom.Address, atom.SizeOfAddress, atom.IsAddressInLittleEndian);

            var name = Encoding.UTF8.GetBytes(atom.Name);
            writer.Write(name, 0, name.Length);
            writer.Write((byte)0x00);
        }

        /// <summary>
        /// Writes an address.
        /// </summary>
        /// <param name="address">The address to write.</param>
        /// <param name="sizeOfAddress">The actual size of <paramref name="address"/> in bytes.</param>
        /// <param name="isAddressInLittleEndian">True if the address should be encoded in little endian.</param>
        private void Write(ulong address, byte sizeOfAddress, bool isAddressInLittleEndian)
        {
            if (sizeOfAddress == 2)
            {
                if (isAddressInLittleEndian)
                    writer.Write((ushort)address);
                else
                    writer.WriteBigEndian((ushort)address);
            }
            else if (sizeOfAddress == 4)
            {
                if (isAddressInLittleEndian)
                    writer.Write((uint)address);
                else
                    writer.WriteBigEndian((uint)address);
            }
            else
            {
                if (isAddressInLittleEndian)
                    writer.Write(address);
                else
                    writer.WriteBigEndian(address);
            }
        }
    }
}