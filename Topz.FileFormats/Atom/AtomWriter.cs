using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Writes Atom object files.
    /// </summary>
    public class AtomWriter
    {
        private BinaryWriter writer;

        private ObjectFile of;

        /// <summary>
        /// Writes an object file to a stream.
        /// </summary>
        /// <param name="file">The object file to write.</param>
        /// <param name="destination">The stream the object file is written to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="file"/> or <paramref name="destination"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="destination"/> is read only.
        /// </exception>
        public void Write(ObjectFile file, Stream destination)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("The stream is read only.", nameof(destination));

            using (writer = new BinaryWriter(destination, Encoding.UTF8, true))
            {
                // Write header.
                writer.Write(0x6D6F7461);   // Ascii for "atom" backwards.
                writer.Write((ushort)1);
                writer.Write(file.IsOriginSet);
                writer.Write(file.Origin);

                of = file;
                foreach (dynamic atom in file)
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
                writer.Write((uint)of.Atoms.IndexOf(reference.Atom));
                writer.Write((byte)reference.AddressType);
                writer.Write(reference.Address);
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

            var name = Encoding.UTF8.GetBytes(atom.Name);
            writer.Write(name, 0, name.Length);
            writer.Write((byte)0x00);
        }
    }
}