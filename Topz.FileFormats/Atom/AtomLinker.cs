using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Links one or more Atom object files.
    /// </summary>
    public class AtomLinker
    {
        private Stream stream;

        private Dictionary<Atom, long> addresses = new Dictionary<Atom, long>();

        private long address;

        /// <summary>
        /// Combines object files into a single one.
        /// </summary>
        /// <param name="files">The object files to combine.</param>
        /// <returns>The combined object file.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="files"/> is null.
        /// </exception>
        /// <exception cref="InvalidObjectFileException">
        /// The combined object file is invalid.
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = Justifications.InstanceAccessMayBeNeededLater)]
        public ObjectFile Link(IEnumerable<ObjectFile> files)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            var combined = new ObjectFile();
            foreach (var file in files)
            {
                if (combined.Origin != file.Origin)
                {
                    if (combined.Origin.HasValue)
                        throw new InvalidObjectFileException("Inconsistent origin.");

                    combined.Origin = file.Origin;
                }

                foreach (var atom in file)
                {
                    var procedure = atom as Procedure;
                    if (procedure != null)
                    {
                        if (combined.OfType<Procedure>().Any(p => p.IsMain) && procedure.IsMain)
                            throw new InvalidObjectFileException("Multiple main procedures.");
                    }

                    var duplicate = combined.FirstOrDefault(a => a.Name == atom.Name);
                    if (duplicate != null)
                    {
                        if (duplicate.IsDefined && atom.IsDefined)
                            throw new InvalidObjectFileException("There are multiple atoms with called '" + atom.Name + "'.");
                        if (duplicate.GetType() != atom.GetType())
                            throw new InvalidObjectFileException("'" + duplicate.Name + "' and '" + atom.Name + "' is not of the same type.");
                        if (!duplicate.IsDefined && !atom.IsDefined)
                            continue;
                        if (duplicate.IsDefined && !atom.IsDefined)
                            continue;

                        combined.Atoms.Remove(duplicate);
                    }

                    combined.Atoms.Add(Copy(atom));
                }
            }

            CopyReferences(files, combined);

            return combined;
        }

        /// <summary>
        /// Links the give object <paramref name="files"/> and writes 
        /// the binary code to the <paramref name="destination"/>.
        /// </summary>
        /// <param name="files">The object files to link</param>
        /// <param name="destination">The stream the binary is written to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="files"/> or <paramref name="destination"/> is null.
        /// </exception>
        /// <exception cref="InvalidObjectFileException">
        /// The combined object file is invalid.
        /// </exception>
        public void Link(IEnumerable<ObjectFile> files, Stream destination)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            stream = destination;
            var file = Link(files);

            var builder = new StringBuilder();
            foreach (var atom in file)
            {
                if (!atom.IsDefined)
                {
                    if (builder.Length == 0)
                        builder.AppendLine("Undefined atoms:");

                    builder.AppendLine("\t" + atom.Name);
                }
            }

            if (builder.Length != 0)
                throw new InvalidObjectFileException(builder.ToString());

            if (!file.OfType<Procedure>().Any(p => p.IsMain))
                throw new InvalidObjectFileException("There is no main procedure.");

            addresses.Clear();
            address = file.Origin.HasValue ? file.Origin.Value : 0L;

            var main = file.OfType<Procedure>().First(p => p.IsMain);
            foreach (dynamic atom in Walk(main))
                Link(atom);

            Resolve(file.OfType<Procedure>());
        }

        /// <summary>
        /// Copies the references from the object <paramref name="files"/>.
        /// </summary>
        /// <param name="files">The object files to copy references from.</param>
        /// <param name="combined">The object file that owns the copied references.</param>
        private static void CopyReferences(IEnumerable<ObjectFile> files, ObjectFile combined)
        {
            foreach (var file in files)
            {
                foreach (var procedure in file.OfType<Procedure>().Where(p => p.IsDefined))
                {
                    foreach (var reference in procedure.References)
                    {
                        var proc = combined.OfType<Procedure>().First(p => p.Name == procedure.Name);
                        if (reference is GlobalReference)
                        {
                            var global = reference as GlobalReference;
                            var referenced = combined.First(a => a.Name == global.Atom.Name);

                            var r = new GlobalReference(referenced);
                            r.Address = global.Address;
                            r.AddressType = global.AddressType;

                            if (!referenced.IsGlobal)
                            {
                                var a = GetDefiningObjectFile(referenced.Name, files);
                                var b = GetDefiningObjectFile(proc.Name, files);

                                if (a != null && b != null && a != b)
                                    throw new InvalidObjectFileException($"'{proc.Name}' is referencing '{referenced.Name}' which is local to another object file.");
                            }

                            proc.References.Add(r);
                        }
                        else
                        {
                            var local = reference as LocalReference;
                            proc.References.Add(new LocalReference()
                            {
                                Address = local.Address,
                                AddressType = local.AddressType,
                                Target = local.Target
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a copy of the given <paramref name="atom"/>.
        /// </summary>
        /// <param name="atom">The atom to copy.</param>
        /// <returns>The copy of the <paramref name="atom"/>.</returns>
        private static Atom Copy(Atom atom)
        {
            return Copy((dynamic)atom);
        }

        /// <summary>
        /// Creates a copy of the given <paramref name="procedure"/>.
        /// </summary>
        /// <param name="procedure">The atom to copy.</param>
        /// <returns>The copy of the <paramref name="procedure"/>.</returns>
        private static Atom Copy(Procedure procedure)
        {
            var copy = new Procedure();
            copy.IsDefined = procedure.IsDefined;
            copy.IsGlobal = procedure.IsGlobal;
            copy.IsMain = procedure.IsMain;
            copy.Name = procedure.Name;

            foreach (var b in procedure.Code)
                copy.Code.Add(b);

            return copy;
        }

        /// <summary>
        /// Creates a copy of the given string.
        /// </summary>
        /// <param name="s">The atom to copy.</param>
        /// <returns>The copy of the <paramref name="s"/>.</returns>
        private static Atom Copy(NullTerminatedString s)
        {
            var copy = new NullTerminatedString();
            copy.IsDefined = s.IsDefined;
            copy.IsGlobal = s.IsGlobal;
            copy.Name = s.Name;
            copy.Content = s.Content;

            return copy;
        }

        /// <summary>
        /// Creates a copy of the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The atom to copy.</param>
        /// <returns>The copy of the <paramref name="data"/>.</returns>
        private static Atom Copy(Data data)
        {
            var copy = new Data();
            copy.IsDefined = data.IsDefined;
            copy.IsGlobal = data.IsGlobal;
            copy.Name = data.Name;

            foreach (var b in data.Content)
                copy.Content.Add(b);

            return copy;
        }

        /// <summary>
        /// Gets the object file that defines an atom with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the atom.</param>
        /// <param name="files">The object files to search in.</param>
        /// <returns>The object file that defines an atom with the <paramref name="name"/>; otherwise null.</returns>
        private static ObjectFile GetDefiningObjectFile(string name, IEnumerable<ObjectFile> files)
        {
            foreach (var file in files)
            {
                foreach (var atom in file)
                {
                    if (atom.IsDefined && atom.Name == name)
                        return file;
                }
            }

            return null;
        }

        /// <summary>
        /// Resolves the referenced atom's addresses in the binary.
        /// </summary>
        /// <param name="procedures">The procedures in the binary.</param>
        private void Resolve(IEnumerable<Procedure> procedures)
        {
            foreach (var procedure in procedures)
            {
                foreach (var reference in procedure.References)
                {
                    var target = 0L;
                    if (reference is GlobalReference)
                        target = addresses[((GlobalReference)reference).Atom];
                    else
                        target = addresses[procedure] + ((LocalReference)reference).Target;

                    var start = addresses[procedure] + reference.Address;
                    stream.Seek(start, SeekOrigin.Begin);

                    var buffer = new byte[4];
                    stream.Read(buffer, 0, buffer.Length);

                    var instruction = BitConverter.ToUInt32(buffer.Reverse().ToArray(), 0);

                    var offset = 0u;
                    switch (reference.AddressType)
                    {
                        case AddressType.ArmOffset12:
                            offset = (uint)Math.Abs(start - target);
                            if (offset > 4095)
                                throw new Exception($"Out of the ±4kB range.");

                            // Set the U bit.
                            instruction = instruction.SetBit(23, start <= target);

                            // Clear offset.
                            instruction &= ~0xFFFu;

                            break;
                        case AddressType.ArmTargetAddress:
                            if (Math.Abs(start - target) > 3.2e+7)
                                throw new Exception($"Out of the ±32MB range.");

                            offset = (uint)((((start <= target ? start + target : target - start) - 8) >> 2) | 0x80000000) & 0x00FFFFFF;

                            // Clear offset.
                            instruction &= 0xFF000000u;

                            break;
                        default:
                            throw new NotSupportedException($"The {reference.Address} address mode is not supported.");
                    }

                    // Use the resolved offset.
                    instruction |= offset;

                    stream.Seek(start, SeekOrigin.Begin);
                    buffer = BitConverter.GetBytes(instruction);

                    stream.Write(buffer.Reverse().ToArray(), 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Links a procedure.
        /// </summary>
        /// <param name="procedure">The procedure to link.</param>
        private void Link(Procedure procedure)
        {
            addresses.Add(procedure, address);
            stream.Write(procedure.Code.ToArray(), 0, procedure.Code.Count);

            address += procedure.Size;
        }

        /// <summary>
        /// Links a chunk of data.
        /// </summary>
        /// <param name="data">The data to link.</param>
        private void Link(Data data)
        {
            addresses.Add(data, address);
            stream.Write(data.Content.ToArray(), 0, data.Content.Count);

            address += data.Size;
        }

        /// <summary>
        /// Links a null terminated string.
        /// </summary>
        /// <param name="s">The string to link.</param>
        private void Link(NullTerminatedString s)
        {
            addresses.Add(s, address);

            var bytes = Encoding.UTF8.GetBytes(s.Content);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0x00);

            address += s.Size + 1;
        }

        /// <summary>
        /// Walks the given <paramref name="procedure"/> and its 
        /// references as a graph.
        /// </summary>
        /// <param name="procedure">The root of the graph.</param>
        /// <returns>
        /// The given <paramref name="procedure"/> and its references recursively.
        /// </returns>
        private IEnumerable<Atom> Walk(Procedure procedure)
        {
            yield return procedure;
            foreach (var reference in procedure.References)
            {
                if (reference is LocalReference)
                    continue;

                var global = reference as GlobalReference;
                if (global.Atom is Procedure)
                {
                    foreach (var atom in Walk(global.Atom as Procedure))
                        yield return atom;
                }
                else
                    yield return global.Atom;
            }
        }
    }
}