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
        /// can read an object file with a prcedure in it.
        /// </summary>
        [Test]
        public void Read_ValidHeaderAndProcedure_ProcedureRead()
        {
            byte[] bytes =
            {
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.

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
                0x01, 0x00, 0x00, 0x00,         // THe size of the code is 1 byte.
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
        /// Tests that <see cref="AtomReader.Read(Stream)"/> throws an 
        /// exception if the stream end unexpectedly right before the
        /// 'is defined' bool.
        /// </summary>
        [Test]
        public void Read_EndOfFileAtIsDefined_ThrowsException()
        {
            byte[] bytes =
            {
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

                0x00,                       // Procedure type.
                0x01,                       // The procedure is defined.
                0x01,                       // The procedure is global.
                0x00,                       // The address is not fixed.
                0x01,                       // The address is in little endian.
                0x00                        // The address size.
            };

            string message = "The address size at 0x0C is invalid. It must be 2, 4 or 8.";
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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                // Base part of the atom.
                0x61, 0x74, 0x6F, 0x6D,     // Magic number.
                0x01, 0x00,                 // Version 1.

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
                0x01, 0x00, 0x00, 0x00      // THe size of the code is 1 byte.
            };

            string message = "Unexpected end of object file. Expected a code block of 0x01 bytes but 0x00 was read.";
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new AtomReader();
                Assert.That(() => reader.Read(stream),
                    Throws.Exception.TypeOf<InvalidObjectFileException>().With.Message.EqualTo(message));
            }
        }
    }
}