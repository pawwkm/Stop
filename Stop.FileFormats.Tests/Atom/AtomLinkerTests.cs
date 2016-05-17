using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stop.FileFormats.Atom
{
    /// <summary>
    /// Provides tests for the <see cref="AtomLinker"/> class.
    /// </summary>
    public class AtomLinkerTests
    {
        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link 2 object files with one atom each.
        /// </summary>
        [Test]
        public void Link_2FilesWith1Atom_LinksObjectFiles()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new NullTerminatedString()
            {
                Content = "Abc",
                IsDefined = true,
                Name = "A"
            });

            var b = new ObjectFile();
            b.Atoms.Add(new NullTerminatedString()
            {
                Content = "Def",
                IsDefined = true,
                Name = "B"
            });

            var linker = new AtomLinker();
            var c = linker.Link(new[] { a, b });

            Assert.AreEqual(2, c.Atoms.Count);

            Assert.AreEqual("A", ((NullTerminatedString)c.Atoms[0]).Name);
            Assert.AreEqual("Abc", ((NullTerminatedString)c.Atoms[0]).Content);
            Assert.True(((NullTerminatedString)c.Atoms[0]).IsDefined);

            Assert.AreEqual("B", ((NullTerminatedString)c.Atoms[1]).Name);
            Assert.AreEqual("Def", ((NullTerminatedString)c.Atoms[1]).Content);
            Assert.True(((NullTerminatedString)c.Atoms[1]).IsDefined);
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link 2 object files. The first object file has a null 
        /// terminated string that is being referenced in the second 
        /// object file by a procedure. The string is global.
        /// </summary>
        [Test]
        public void Link_GlobalAtomInFile1ReferencedByAtomInFile2_LinksObjectFiles()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new NullTerminatedString()
            {
                IsDefined = true,
                IsGlobal = true,
                Content = "Abc",
                Name = "A"
            });

            var b = new ObjectFile();
            b.Atoms.Add(new NullTerminatedString()
            {
                Name = "A"
            });

            var procedure = new Procedure();
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.References.Add(new Reference(b.Atoms[0]));
            b.Atoms.Add(procedure);

            var linker = new AtomLinker();
            var c = linker.Link(new[] { a, b });

            Assert.AreEqual(2, c.Atoms.Count);

            Assert.AreEqual("A", ((NullTerminatedString)c.Atoms[0]).Name);
            Assert.AreEqual("Abc", ((NullTerminatedString)c.Atoms[0]).Content);
            Assert.True(((NullTerminatedString)c.Atoms[0]).IsDefined);

            Assert.AreEqual("Proc", ((Procedure)c.Atoms[1]).Name);
            Assert.AreSame(((Procedure)c.Atoms[1]).References[0].Atom, c.Atoms[0]);
            Assert.True(((Procedure)c.Atoms[1]).IsDefined);
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link 2 object files. The first object file has a procedure 
        /// that references a null terminated string in the second object file.
        /// The string is global.
        /// </summary>
        [Test]
        public void Link_GlobalAtomInFile2ReferencedByAtomInFile1_LinksObjectFiles()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new NullTerminatedString()
            {
                Name = "A"
            });

            var procedure = new Procedure();
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.References.Add(new Reference(a.Atoms[0]));
            a.Atoms.Add(procedure);

            var b = new ObjectFile();
            b.Atoms.Add(new NullTerminatedString()
            {
                IsDefined = true,
                IsGlobal = true,
                Content = "Abc",
                Name = "A"
            });

            var linker = new AtomLinker();
            var c = linker.Link(new[] { a, b });

            Assert.AreEqual(2, c.Atoms.Count);

            Assert.AreEqual("Proc", ((Procedure)c.Atoms[0]).Name);
            Assert.AreSame(((Procedure)c.Atoms[0]).References[0].Atom, c.Atoms[1]);
            Assert.True(((Procedure)c.Atoms[0]).IsDefined);

            Assert.AreEqual("A", ((NullTerminatedString)c.Atoms[1]).Name);
            Assert.AreEqual("Abc", ((NullTerminatedString)c.Atoms[1]).Content);
            Assert.True(((NullTerminatedString)c.Atoms[1]).IsDefined);
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link a main procedure that references a null terminated string.
        /// </summary>
        [Test]
        public void Link_ProcedureReferencingNullTerminatedString_LinksToBinary()
        {
            var file = new ObjectFile();
            file.Atoms.Add(new NullTerminatedString()
            {
                IsDefined = true,
                Content = "Abc",
                Name = "A"
            });

            var procedure = new Procedure();
            file.Atoms.Add(procedure);

            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new Reference(file.Atoms[0])
            {
                IsAddressInLittleEndian = true,
                SizeOfAddress = 2
            });

            var binary = new byte[] 
            {
                0x02, 0x00,             // Procedure code.
                0x41, 0x62, 0x63, 0x00  // The string 'Abc'.
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link a main procedure that references a chunk of data.
        /// </summary>
        [Test]
        public void Link_ProcedureReferencingData_LinksToBinary()
        {
            var file = new ObjectFile();

            var data = new Data();
            file.Atoms.Add(data);

            data.IsDefined = true;
            data.Name = "Data";
            data.Content.Add(0xAA);
            data.Content.Add(0x55);

            var procedure = new Procedure();
            file.Atoms.Add(procedure);

            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new Reference(data)
            {
                IsAddressInLittleEndian = true,
                SizeOfAddress = 2
            });

            var binary = new byte[]
            {
                0x02, 0x00, // Procedure code.
                0xAA, 0x55, // The data chunk.
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link a main procedure that references another procedure.
        /// </summary>
        [Test]
        public void Link_ProcedureReferencingProcedure_LinksToBinary()
        {
            var file = new ObjectFile();

            var sub = new Procedure();
            file.Atoms.Add(sub);

            sub.IsDefined = true;
            sub.Name = "Sub";
            sub.Code.Add(0xAA);
            sub.Code.Add(0x55);

            var main = new Procedure();
            file.Atoms.Add(main);

            main.IsMain = true;
            main.IsDefined = true;
            main.Name = "Proc";
            main.Code.Add(0x00);
            main.Code.Add(0x00);

            main.References.Add(new Reference(sub)
            {
                IsAddressInLittleEndian = true,
                SizeOfAddress = 2
            });

            var binary = new byte[]
            {
                0x02, 0x00, // Main procedure.
                0xAA, 0x55, // Sub procedure.
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// can link while removing dead code.
        /// </summary>
        [Test]
        public void Link_ObjectFileWithMainAndUnreferencedAtoms_OnlyMainIsLinked()
        {
            var file = new ObjectFile();

            var sub = new Procedure();
            file.Atoms.Add(sub);

            sub.IsDefined = true;
            sub.Name = "Sub";
            sub.Code.Add(0xAA);
            sub.Code.Add(0x55);

            var main = new Procedure();
            file.Atoms.Add(main);

            main.IsMain = true;
            main.IsDefined = true;
            main.Name = "Proc";
            main.Code.Add(0x00);
            main.Code.Add(0x00);

            var binary = new byte[]
            {
                0x00, 0x00, // Main procedure.
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// throws an exception if the first object file has a null 
        /// terminated string that is being referenced in the second 
        /// object file by a procedure. The string is local.
        /// </summary>
        [Test]
        public void Link_LocalAtomInFile1ReferencedByAtomInFile2_ThrowsException()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new NullTerminatedString()
            {
                IsDefined = true,
                Content = "Abc",
                Name = "A"
            });

            var b = new ObjectFile();
            b.Atoms.Add(new NullTerminatedString()
            {
                Name = "A"
            });

            var procedure = new Procedure();
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.References.Add(new Reference(b.Atoms[0]));
            b.Atoms.Add(procedure);

            var linker = new AtomLinker();

            var message = "'Proc' is referencing 'A' which is local to another object file.";
            Assert.That(
                () => linker.Link(new[] { a, b }),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// throws an exception if the first object file has a procedure 
        /// that references a null terminated string in the second object file.
        /// The string is local.
        /// </summary>
        [Test]
        public void Link_LocalAtomInFile2ReferencedByAtomInFile1_ThrowsException()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new NullTerminatedString()
            {
                Name = "A"
            });

            var procedure = new Procedure();
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.References.Add(new Reference(a.Atoms[0]));
            a.Atoms.Add(procedure);

            var b = new ObjectFile();
            b.Atoms.Add(new NullTerminatedString()
            {
                IsDefined = true,
                Content = "Abc",
                Name = "A"
            });

            var linker = new AtomLinker();

            var message = "'Proc' is referencing 'A' which is local to another object file.";
            Assert.That(
                () => linker.Link(new[] { a, b }),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// throws an exception if there are inconsistent origins in 
        /// the given object files.
        /// </summary>
        [Test]
        public void Link_InconsistentOrigin_ThrowsException()
        {
            var a = new ObjectFile();
            a.IsOriginSet = true;
            a.Origin = 0x10;

            var b = new ObjectFile();
            b.IsOriginSet = true;

            var linker = new AtomLinker();

            var message = "Inconsistent origin.";
            Assert.That(
                () => linker.Link(new[] { a, b }),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// throws an exception if there are multiple main procedures.
        /// </summary>
        [Test]
        public void Link_MultipleMains_ThrowsException()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new Procedure()
            {
                Name = "Proc1",
                IsMain = true
            });

            var b = new ObjectFile();
            b.Atoms.Add(new Procedure()
            {
                Name = "Proc2",
                IsMain = true
            });

            var linker = new AtomLinker();

            var message = "Multiple main procedures.";
            Assert.That(
                () => linker.Link(new[] { a, b }),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// throws an exception if there are multiple atoms with the same name.
        /// </summary>
        [Test]
        public void Link_MultipleDefinedAtomsWithTheSameName_ThrowsException()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new Procedure()
            {
                Name = "Proc",
                IsDefined = true
            });

            var b = new ObjectFile();
            b.Atoms.Add(new Procedure()
            {
                Name = "Proc",
                IsDefined = true
            });

            var linker = new AtomLinker();

            var message = "There are multiple atoms with called 'Proc'.";
            Assert.That(
                () => linker.Link(new[] { a, b }),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile})"/>
        /// throws an exception if there are multiple atoms with the same name that 
        /// are undefined, but they are of a different type.
        /// </summary>
        [Test]
        public void Link_MultipleUndefinedDifferentlyTypedAtomsWithTheSameName_ThrowsException()
        {
            var a = new ObjectFile();
            a.Atoms.Add(new Procedure()
            {
                Name = "Proc"
            });

            var b = new ObjectFile();
            b.Atoms.Add(new Data()
            {
                Name = "Proc"
            });

            var linker = new AtomLinker();

            var message = "'Proc' and 'Proc' is not of the same type.";
            Assert.That(
                () => linker.Link(new[] { a, b }),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// throws an exception if there are one or more undefined atoms.
        /// </summary>
        [Test]
        public void Link_UndefinedAtoms_ThrowsException()
        {
            var a = new ObjectFile();

            var b = new ObjectFile();
            b.Atoms.Add(new NullTerminatedString()
            {
                Name = "A"
            });

            var procedure = new Procedure();
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            procedure.References.Add(new Reference(b.Atoms[0]));
            b.Atoms.Add(procedure);

            var linker = new AtomLinker();

            var message = new StringBuilder();
            message.AppendLine("Undefined atoms:");
            message.AppendLine("\tA");

            Assert.That(
                () => linker.Link(new[] { a, b }, new MemoryStream()),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message.ToString()));
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// throws an exception if there is no main procedure.
        /// </summary>
        [Test]
        public void Link_NoMainProcedure_ThrowsException()
        {
            var a = new ObjectFile();
            var linker = new AtomLinker();

            var message = "There is no main procedure.";

            Assert.That(
                () => linker.Link(new[] { a }, new MemoryStream()),
                Throws.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
        }
    }
}