using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Topz.FileFormats.Atom
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
            procedure.References.Add(new GlobalReference(b.Atoms[0]));
            b.Atoms.Add(procedure);

            var linker = new AtomLinker();
            var c = linker.Link(new[] { a, b });

            Assert.AreEqual(2, c.Atoms.Count);

            Assert.AreEqual("A", ((NullTerminatedString)c.Atoms[0]).Name);
            Assert.AreEqual("Abc", ((NullTerminatedString)c.Atoms[0]).Content);
            Assert.True(((NullTerminatedString)c.Atoms[0]).IsDefined);

            Assert.AreEqual("Proc", ((Procedure)c.Atoms[1]).Name);
            Assert.AreSame(((GlobalReference)((Procedure)c.Atoms[1]).References[0]).Atom, c.Atoms[0]);
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
            procedure.References.Add(new GlobalReference(a.Atoms[0]));
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
            Assert.AreSame(((GlobalReference)((Procedure)c.Atoms[0]).References[0]).Atom, c.Atoms[1]);
            Assert.True(((Procedure)c.Atoms[0]).IsDefined);

            Assert.AreEqual("A", ((NullTerminatedString)c.Atoms[1]).Name);
            Assert.AreEqual("Abc", ((NullTerminatedString)c.Atoms[1]).Content);
            Assert.True(((NullTerminatedString)c.Atoms[1]).IsDefined);
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve a local reference to a future instruction.
        /// </summary>
        [Test]
        public void Link_ProcedureReferencingLocalFutureInstruction_ResolvesReference()
        {
            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";

            // ldr r0, [r1, #0]
            procedure.Code.Add(0xE5);
            procedure.Code.Add(0x91);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            // Some random instruction to be referenced.
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new LocalReference()
            {
                Address = 0,
                AddressType = AddressType.ArmOffset12,
                Target = 4
            });

            var file = new ObjectFile();
            file.Atoms.Add(procedure);

            var binary = new byte[]
            {
                // ldr r0, [r1, #4]
                0xE5, 0x91, 0x00, 0x04,

                // The random instruction that was referenced.
                0x00, 0x00, 0x00, 0x00
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve an instruction referencing itself.
        /// </summary>
        [Test]
        public void Link_InstructionReferencingItself_ResolvesReference()
        {
            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";

            // Some random instruction.
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            // ldr r0, [r1, #4]
            procedure.Code.Add(0xE5);
            procedure.Code.Add(0x91);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x04);

            procedure.References.Add(new LocalReference()
            {
                Address = 4,
                AddressType = AddressType.ArmOffset12,
                Target = 4
            });

            var file = new ObjectFile();
            file.Atoms.Add(procedure);

            var binary = new byte[]
            {
                // The random instruction that was referenced.
                0x00, 0x00, 0x00, 0x00,

                // ldr r0, [r1, #0]
                0xE5, 0x91, 0x00, 0x00
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve a global <see cref="AddressType.ArmOffset12"/> reference.
        /// </summary>
        [Test]
        public void Link_GlobalOffset12Reference_ResolvesReference()
        {
            var file = new ObjectFile();
            file.Atoms.Add(new NullTerminatedString()
            {
                IsDefined = true,
                Content = "Abc",
                Name = "A"
            });

            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";
            file.Atoms.Add(procedure);

            // ldr r0, [r1, #0]
            procedure.Code.Add(0xE5);
            procedure.Code.Add(0x91);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new GlobalReference(file.Atoms.First())
            {
                Address = 0,
                AddressType = AddressType.ArmOffset12
            });

            var binary = new byte[]
            {
                // ldr r0, [r1, #4]
                0xE5, 0x91, 0x00, 0x04,

                // The "Abc" string.
                0x41, 0x62, 0x63, 0x00
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }
        
        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve a local reference to a past instruction.
        /// </summary>
        [Test]
        public void Link_ProcedureReferencingLocalPastInstruction_ResolvesReference()
        {
            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";

            // Some random instruction to be referenced.
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            // ldr r0, [r1, #0]
            procedure.Code.Add(0xE5);
            procedure.Code.Add(0x91);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new LocalReference()
            {
                Address = 4,
                AddressType = AddressType.ArmOffset12,
                Target = 0
            });

            var file = new ObjectFile();
            file.Atoms.Add(procedure);

            var binary = new byte[]
            {
                // The random instruction that was referenced.
                0x00, 0x00, 0x00, 0x00,

                // ldr r0, [r1, #-4]
                0xE5, 0x11, 0x00, 0x04
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve a local <see cref="AddressType.ArmTargetAddress"/> to a past instruction.
        /// </summary>
        [Test]
        public void Link_ProcedureJumpToPast_ResolvesReference()
        {
            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";

            // Some random instruction to be referenced.
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            // bl #??????
            procedure.Code.Add(0xEB);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new LocalReference()
            {
                Address = 4,
                AddressType = AddressType.ArmTargetAddress,
                Target = 0
            });

            var file = new ObjectFile();
            file.Atoms.Add(procedure);

            var binary = new byte[]
            {
                // The random instruction that was referenced.
                0x00, 0x00, 0x00, 0x00,

                // bl #-4
                0xEB, 0xFF, 0xFF, 0xFD
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve a local <see cref="AddressType.ArmTargetAddress"/> to a future instruction.
        /// </summary>
        [Test]
        public void Link_ProcedureJumpToFuture_ResolvesReference()
        {
            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";

            // bl #??????
            procedure.Code.Add(0xEB);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            // Some random instruction to be referenced.
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new LocalReference()
            {
                Address = 0,
                AddressType = AddressType.ArmTargetAddress,
                Target = 4
            });

            var file = new ObjectFile();
            file.Atoms.Add(procedure);

            var binary = new byte[]
            {
                // bl #4
                0xEB, 0xFF, 0xFF, 0xFF,

                // The random instruction that was referenced.
                0x00, 0x00, 0x00, 0x00
            };

            using (var stream = new MemoryStream())
            {
                var linker = new AtomLinker();
                linker.Link(new[] { file }, stream);

                CollectionAssert.AreEqual(binary, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomLinker.Link(IEnumerable{ObjectFile}, Stream)"/>
        /// can resolve a local <see cref="AddressType.ArmTargetAddress"/> to itself.
        /// </summary>
        [Test]
        public void Link_ProcedureJumpToSelf_ResolvesReference()
        {
            var procedure = new Procedure();
            procedure.IsMain = true;
            procedure.IsDefined = true;
            procedure.Name = "Proc";

            // bl #??????
            procedure.Code.Add(0xEB);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);

            procedure.References.Add(new LocalReference()
            {
                Address = 0,
                AddressType = AddressType.ArmTargetAddress,
                Target = 0
            });

            var file = new ObjectFile();
            file.Atoms.Add(procedure);

            var binary = new byte[]
            {
                // bl #0
                0xEB, 0xFF, 0xFF, 0xFE,
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
            procedure.References.Add(new GlobalReference(b.Atoms[0]));
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
            procedure.References.Add(new GlobalReference(a.Atoms[0]));
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
            a.Origin = 0x10;

            var b = new ObjectFile();
            b.Origin = 0;

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
            procedure.References.Add(new GlobalReference(b.Atoms[0]));
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