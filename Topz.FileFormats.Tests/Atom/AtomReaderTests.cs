using NUnit.Framework;
using System.IO;
using Pote;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Provides tests for the <see cref="AtomReader"/> class.
    /// </summary>
    public class AtomReaderTests
    {
        /// <summary>
        /// Tests that  <see cref="AtomReader.Read(Stream)"/>
        /// can read an object file with no atoms in it.
        /// </summary>
        [Test]
        public void Read_ValidHeader_NoAtomsRead()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,  // Magic number.
                0x01, 0x00,              // Version 1.
                0x01,                    // The origin is set.
                0x20, 0x00, 0x00, 0x00,  // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var file = reader.Read(stream);

                Assert.AreEqual(0, file.Atoms.Count);
                Assert.True(file.IsOriginSet);
                Assert.AreEqual(0x20, file.Origin);
            }
        }

        /// <summary>
        /// Tests that  <see cref="AtomReader.Read(Stream)"/>
        /// can read an object file with a procedure in it.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndProcedure_ProcedureRead()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x50, 0x72, 0x6F, 0x63, 0x00,   // The procedure is called 'Proc'.

                // Procedure part of the atom.
                0x01,                           // This is the main procedure.
                0x01, 0x00, 0x00, 0x00,         // The size of the code is 1 byte.
                0xAA,                           // The procedure code.
                0x00, 0x00                      // Number of references.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var file = reader.Read(stream);

                Assert.True(file.IsOriginSet);
                Assert.AreEqual(0x20, file.Origin);

                var procedure = (Procedure)file.Atoms[0];

                Assert.True(procedure.IsDefined);
                Assert.True(procedure.IsGlobal);
                Assert.AreEqual("Proc", procedure.Name);

                Assert.True(procedure.IsMain);
                CollectionAssert.AreEqual(new byte[] { 0xAA }, procedure.Code);
                Assert.AreEqual(0, procedure.References.Count);
            }
        }

        /// <summary>
        /// Tests that  <see cref="AtomReader.Read(Stream)"/>
        /// can read an object file with a procedure in it.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndProcedureWithReference_ProcedureRead()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x50, 0x72, 0x6F, 0x63, 0x00,   // The procedure is called 'Proc'.

                // Procedure part of the atom.
                0x01,                           // This is the main procedure.
                0x04, 0x00, 0x00, 0x00,         // The size of the code is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The procedure code.
                0x01, 0x00,                     // Number of references.

                // Reference.
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The address to relocate.

                // Null terminated string.
                0x01,                           // Null terminated string type.
                0x01,                           // The string is defined.
                0x00,                           // The string is not global.
                0x53, 0x00,                     // The string is called 'S'.

                // String part
                0x54, 0x78, 0x74, 0x00          // The string is 'Txt'.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var file = reader.Read(stream);

                Assert.True(file.IsOriginSet);
                Assert.AreEqual(0x20, file.Origin);

                var procedure = (Procedure)file.Atoms[0];
                var s = (NullTerminatedString)file.Atoms[1];

                Assert.True(procedure.IsDefined);
                Assert.True(procedure.IsGlobal);
                Assert.AreEqual("Proc", procedure.Name);

                Assert.True(procedure.IsMain);
                CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, procedure.Code);

                Assert.AreEqual(1, procedure.References.Count);
                Assert.AreEqual(0, procedure.References[0].Address);
                Assert.AreSame(s, procedure.References[0].Atom);

                Assert.True(s.IsDefined);
                Assert.False(s.IsGlobal);
                Assert.AreEqual("S", s.Name);
                Assert.AreEqual("Txt", s.Content);
            }
        }

        /// <summary>
        /// Tests that  <see cref="AtomReader.Read(Stream)"/>
        /// can read an object file with a data block in it.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndData_DataRead()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x44, 0x61, 0x74, 0x61, 0x00,   // The data is called 'Data'.

                // Data part of the atom.
                0x01, 0x00, 0x00, 0x00,         // The size of the data block is 1 byte.
                0xAA                            // The data block.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var file = reader.Read(stream);

                Assert.True(file.IsOriginSet);
                Assert.AreEqual(0x20, file.Origin);

                var data = (Data)file.Atoms[0];

                Assert.True(data.IsDefined);
                Assert.True(data.IsGlobal);
                Assert.AreEqual("Data", data.Name);

                CollectionAssert.AreEqual(new byte[] { 0xAA }, data.Content);
            }
        }

        /// <summary>
        /// Tests that  <see cref="AtomReader.Read(Stream)"/>
        /// can read an object file with a null terminated string in it.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndNullTerminatedString_StringRead()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x01,                           // String type.
                0x01,                           // The string is defined.
                0x01,                           // The string is global.
                0x54, 0x78, 0x74, 0x00,         // The string is called 'Txt'.

                // String part of the atom.
                0x41, 0x62, 0x63, 0x00          // The string is 'Abc'.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var file = reader.Read(stream);

                Assert.True(file.IsOriginSet);
                Assert.AreEqual(0x20, file.Origin);

                var s = (NullTerminatedString)file.Atoms[0];

                Assert.True(s.IsDefined);
                Assert.True(s.IsGlobal);
                Assert.AreEqual("Txt", s.Name);

                CollectionAssert.AreEqual("Abc", s.Content);
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if a procedure has references with addresses 
        /// that starts at the same place.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndProcedureWithReferencesAtTheSameAddress_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x50, 0x72, 0x6F, 0x63, 0x00,   // The procedure is called 'Proc'.

                // Procedure part of the atom.
                0x01,                           // This is the main procedure.
                0x04, 0x00, 0x00, 0x00,         // The size of the code is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The procedure code.
                0x02, 0x00,                     // Number of references.

                // Reference.
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The address to relocate.

                // Reference.
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The address to relocate.

                // Null terminated string.
                0x01,                           // Null terminated string type.
                0x01,                           // The string is defined.
                0x00,                           // The string is not global.
                0x53, 0x00,                     // The string is called 'S'.

                // String part
                0x54, 0x78, 0x74, 0x00          // The string is 'Txt'.
            };

            string message = "Proc's reference to 'S' has an overlapping address with the reference to 'S' at 0x00.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if a procedure has an 'old' reference that 
        /// overlaps a 'new' one.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndProcedureWithOldRefrenceOverlappingANewOne_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x50, 0x72, 0x6F, 0x63, 0x00,   // The procedure is called 'Proc'.

                // Procedure part of the atom.
                0x01,                           // This is the main procedure.
                0x05, 0x00, 0x00, 0x00,         // The size of the code is 5 bytes.
                0x00, 0x00, 0x00, 0x00,  0x00,  // The procedure code.
                0x02, 0x00,                     // Number of references.

                // Old Reference. 
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The address to relocate.

                // New Reference.
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x01, 0x00, 0x00, 0x00,         // The address to relocate.

                // Null terminated string.
                0x01,                           // Null terminated string type.
                0x01,                           // The string is defined.
                0x00,                           // The string is not global.
                0x53, 0x00,                     // The string is called 'S'.

                // String part
                0x54, 0x78, 0x74, 0x00          // The string is 'Txt'.
            };

            string message = "Proc's reference to 'S' has an overlapping address with the reference to 'S' at 0x00.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if a procedure has an 'new' reference that 
        /// overlaps a 'old' one.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndProcedureWithNewRefrenceOverlappingAnOldOne_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x50, 0x72, 0x6F, 0x63, 0x00,   // The procedure is called 'Proc'.

                // Procedure part of the atom.
                0x01,                           // This is the main procedure.
                0x05, 0x00, 0x00, 0x00,         // The size of the code is 5 bytes.
                0x00, 0x00, 0x00, 0x00,  0x00,  // The procedure code.
                0x02, 0x00,                     // Number of references.

                // Old Reference. 
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x01, 0x00, 0x00, 0x00,         // The address to relocate.

                // New Reference.
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x00, 0x00, 0x00, 0x00,         // The address to relocate.

                // Null terminated string.
                0x01,                           // Null terminated string type.
                0x01,                           // The string is defined.
                0x00,                           // The string is not global.
                0x53, 0x00,                     // The string is called 'S'.

                // String part
                0x54, 0x78, 0x74, 0x00          // The string is 'Txt'.
            };

            string message = "Proc's reference to 'S' has an overlapping address with the reference to 'S' at 0x01.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'is defined' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtIsDefined_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00                        // Procedure type.
            };

            string message = "Unexpected end of object file. Expected 'is defined' bool.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'is global' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtIsGlobal_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01                        // The procedure is defined.
            };

            string message = "Unexpected end of object file. Expected 'is global' bool.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'is main' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtIsMain_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00                  // The procedure is called 'P'.
            };

            string message = "Unexpected end of object file. Expected 'is main' bool.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'code block size' integer.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtCodeBlockSize_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01                        // This is the main procedure.
            };

            string message = "Unexpected end of object file. Expected the 32 bit number for code size.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'code block' bytes.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtCodeBlock_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01,                       // This is the main procedure.
                0x01, 0x00, 0x00, 0x00      // The size of the code is 1 byte.
            };

            string message = "Unexpected end of object file. Expected a code block of 0x01 bytes but 0x00 was read.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'reference count' integer.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtReferenceCount_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01,                       // This is the main procedure.
                0x01, 0x00, 0x00, 0x00,     // The size of the code is 1 byte.
                0xAA                        // Code block.
            };

            string message = "Unexpected end of object file. Expected the 16 bit number for amount of references.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before a
        /// reference struct.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtReference_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01,                       // This is the main procedure.
                0x01, 0x00, 0x00, 0x00,     // The size of the code is 1 byte.
                0xAA,                       // Code block.
                0x01, 0x00                  // One reference.
            };

            string message = "Unexpected end of object file. Expected the number of an atom.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'is address in little endian' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtReferenceIsLittleEndian_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01,                       // This is the main procedure.
                0x01, 0x00, 0x00, 0x00,     // The size of the code is 1 byte.
                0xAA,                       // Code block.
                0x01, 0x00,                 // One reference.
                0x01, 0x00, 0x00, 0x00      // Refer to atom number 1.
            };

            string message = "Unexpected end of object file. Expected 'is little endian' bool.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'address size' byte.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtReferenceAddressSize_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01,                       // This is the main procedure.
                0x01, 0x00, 0x00, 0x00,     // The size of the code is 1 byte.
                0xAA,                       // Code block.
                0x01, 0x00,                 // One reference.
                0x01, 0x00, 0x00, 0x00,     // Refer to atom number 1.
                0x01                        // The address is in little endian.
            };

            string message = "Unexpected end of object file. Expected 'address size' byte.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if a reference has an address size that isn't 
        /// 2 or 4.
        /// </summary>
        [Test]
        public void Read_ReferenceAddressSizeIsNot2Or4_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.
                0x01,                       // The origin is set.
                0x20, 0x00, 0x00, 0x00,     // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x50, 0x00,                 // The procedure is called 'P'.

                // Procedure part of the atom.
                0x01,                       // This is the main procedure.
                0x01, 0x00, 0x00, 0x00,     // The size of the code is 1 byte.
                0xAA,                       // Code block.
                0x01, 0x00,                 // One reference.
                0x01, 0x00, 0x00, 0x00,     // Refer to atom number 1.
                0x01,                       // The address is in little endian.
                0x00                        // Invalid address size
            };

            string message = "The address size at 0x21 is invalid. It must be 2 or 4.";
            for (int i = 0; i < byte.MaxValue; i++)
            {
                if (i.IsOneOf(2, 4))
                    continue;

                bytes[bytes.Length - 1] = (byte)i;
                using (var stream = new MemoryStream(bytes))
                {
                    var reader = new AtomReader();
                    Assert.That(() => reader.Read(stream),
                        Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'data size' integer.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtDataBlockSize_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x44, 0x61, 0x74, 0x61, 0x00,   // The block is called 'Data'.
            };

            string message = "Unexpected end of object file. Expected a the size of a data block.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// data block.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtDataBlock_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x44, 0x61, 0x74, 0x61, 0x00,   // The block is called 'Data'.

                // Data part of the atom.
                0x02, 0x00, 0x00, 0x00          // The block consist of 2 bytes of data.
            };

            string message = "Unexpected end of object file. Expected a data block of 0x02 bytes but 0x00 was read.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly in the data block.
        /// </summary>
        [Test]
        public void Read_EndOfFileInDataBlock_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x44, 0x61, 0x74, 0x61, 0x00,   // The block is called 'Data'.

                // Data part of the atom.
                0x02, 0x00, 0x00, 0x00,         // The block consist of 2 bytes of data.
                0xAA
            };

            string message = "Unexpected end of object file. Expected a data block of 0x02 bytes but 0x01 was read.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// null terminated string.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtNullTerminatedString_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,

                // Base part of the atom.
                0x01,                           // String type.
                0x01,                           // The string is defined.
                0x01,                           // The string is global.
                0x53, 0x00                      // The string is called 'S'.
            };

            string message = "Unexpected end of object file. Expected null terminated string.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }
    }
}