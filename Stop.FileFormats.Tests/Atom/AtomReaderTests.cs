using NUnit.Framework;
using System.IO;
using Pote;

namespace Stop.FileFormats.Atom
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
                0x01, 0x00               // Version 1.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var atoms = reader.Read(stream);

                Assert.AreEqual(0, atoms.Count);
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

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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
                var atoms = reader.Read(stream);

                var procedure = (Procedure)atoms[0];

                Assert.True(procedure.IsDefined);
                Assert.True(procedure.IsGlobal);
                Assert.False(procedure.IsAddressFixed);
                Assert.True(procedure.IsAddressInLittleEndian);
                Assert.AreEqual(0x0020, procedure.Address);
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

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x40, 0x00, 0x00, 0x00,         // Absolute address is 0x00000040.
                0x53, 0x00,                     // The string is called 'S'.

                // String part
                0x54, 0x78, 0x74, 0x00          // The string is 'Txt'.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var atoms = reader.Read(stream);

                var procedure = (Procedure)atoms[0];
                var s = (NullTerminatedString)atoms[1];

                Assert.True(procedure.IsDefined);
                Assert.True(procedure.IsGlobal);
                Assert.False(procedure.IsAddressFixed);
                Assert.True(procedure.IsAddressInLittleEndian);
                Assert.AreEqual(0x0020, procedure.Address);
                Assert.AreEqual("Proc", procedure.Name);

                Assert.True(procedure.IsMain);
                CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }, procedure.Code);

                Assert.AreEqual(1, procedure.References.Count);
                Assert.AreEqual(0, procedure.References[0].Address);
                Assert.AreSame(s, procedure.References[0].Atom);

                Assert.True(s.IsDefined);
                Assert.False(s.IsGlobal);
                Assert.False(s.IsAddressFixed);
                Assert.True(s.IsAddressInLittleEndian);
                Assert.AreEqual(0x0040, s.Address);
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

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
                0x44, 0x61, 0x74, 0x61, 0x00,   // The data is called 'Data'.

                // Data part of the atom.
                0x01, 0x00, 0x00, 0x00,         // The size of the data block is 1 byte.
                0xAA                            // The data block.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var atoms = reader.Read(stream);

                var data = (Data)atoms[0];

                Assert.True(data.IsDefined);
                Assert.True(data.IsGlobal);
                Assert.False(data.IsAddressFixed);
                Assert.True(data.IsAddressInLittleEndian);
                Assert.AreEqual(0x0020, data.Address);
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

                // Base part of the atom.
                0x01,                           // String type.
                0x01,                           // The string is defined.
                0x01,                           // The string is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
                0x54, 0x78, 0x74, 0x00,         // The string is called 'Txt'.

                // String part of the atom.
                0x41, 0x62, 0x63, 0x00          // The string is 'Abc'.
            };

            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                var atoms = reader.Read(stream);

                var s = (NullTerminatedString)atoms[0];

                Assert.True(s.IsDefined);
                Assert.True(s.IsGlobal);
                Assert.False(s.IsAddressFixed);
                Assert.True(s.IsAddressInLittleEndian);
                Assert.AreEqual(0x0020, s.Address);
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

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x40, 0x00, 0x00, 0x00,         // Absolute address is 0x00000040.
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

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x40, 0x00, 0x00, 0x00,         // Absolute address is 0x00000040.
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

                // Base part of the atom.
                0x00,                           // Procedure type.
                0x01,                           // The procedure is defined.
                0x01,                           // The procedure is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x40, 0x00, 0x00, 0x00,         // Absolute address is 0x00000040.
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
        /// 'is address fixed' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtIsAddressFixed_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01                        // The procedure is global.
            };

            string message = "Unexpected end of object file. Expected 'is address fixed' bool.";
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
        public void Read_EndOfFileAtIsLittleEndian_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00                        // The address is not fixed.
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
        /// 'address size' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtSizeOfAddress_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
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
        /// exception if the 'address size' byte is not 2, 4 or 8.
        /// </summary>
        [Test]
        public void Read_SizeOfAddressIsNot2Or4Or8_ThrowsException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x00                        // The address size.
            };

            string message = "The address size at 0x0B is invalid. It must be 2, 4 or 8.";
            for (int i = 0; i < byte.MaxValue; i++)
            {
                if (i.IsOneOf(2, 4, 8))
                    continue;

                bytes[11] = (byte)i;
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
        /// 'address' integer.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtAtomAddressAndExpectedAnAddressOf2Bytes_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02                        // The address size.
            };

            string message = "Unexpected end of object file. Expected a 16 bit address.";
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
        /// 'address' integer.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtAtomAddressAndExpectedAnAddressOf4Bytes_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x04                        // The address size.
            };

            string message = "Unexpected end of object file. Expected a 32 bit address.";
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
        /// 'address' integer.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtAtomAddressAndExpectedAnAddressOf8Bytes_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x08                        // The address size.
            };

            string message = "Unexpected end of object file. Expected a 64 bit address.";
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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
        /// exception if the stream end unexpectedly right before the
        /// 'address size' byte.
        /// </summary>
        [Test]
        public void Read_ReferenceAddressSizeIsNot2Or4Or8_ThrowException()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                // Base part of the atom.
                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x02,                       // The address size.
                0x20, 0x00,                 // Absolute address is 0x0020.
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

            string message = "The address size at 0x1D is invalid. It must be 2 or 4.";
            for (int i = 0; i < byte.MaxValue; i++)
            {
                if (i.IsOneOf(2, 4))
                    continue;

                bytes[29] = (byte)i;
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

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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

                // Base part of the atom.
                0x02,                           // Data type.
                0x01,                           // The data is defined.
                0x01,                           // The data is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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

                // Base part of the atom.
                0x01,                           // String type.
                0x01,                           // The string is defined.
                0x01,                           // The string is global.
                0x00,                           // The address is not fixed.
                0x01,                           // The address is in little endian.
                0x04,                           // The size of the address is 4 bytes.
                0x20, 0x00, 0x00, 0x00,         // Absolute address is 0x00000020.
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