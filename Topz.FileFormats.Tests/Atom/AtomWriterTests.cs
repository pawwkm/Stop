using NUnit.Framework;
using System.IO;

namespace Topz.FileFormats.Atom
{
    /// <summary>
    /// Provides tests for the <see cref="AtomWriter"/> class.
    /// </summary>
    public class AtomWriterTests
    {
        /// <summary>
        /// Tests that <see cref="AtomWriter.Write(ObjectFile, Stream)"/>
        /// writes only the header if there are no atoms.
        /// </summary>
        [Test]
        public void Write_NoAtoms_HeaderWritten()
        {
            byte[] bytes =
            {
                0x61, 0x74, 0x6F, 0x6D,         // Magic number.
                0x01, 0x00,                     // Version 1.
                0x01,                           // The origin is set.
                0x20, 0x00, 0x00, 0x00,         // The origin is set to 0x20.
                0x00, 0x00, 0x00, 0x00,
            };

            using (var stream = new MemoryStream())
            {
                var writer = new AtomWriter();

                var file = new ObjectFile();
                file.Origin = 0x20;

                writer.Write(file, stream);

                CollectionAssert.AreEqual(bytes, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomWriter.Write(ObjectFile, Stream)"/>
        /// can write a procedure without references.
        /// </summary>
        [Test]
        public void Write_ProcedureWithoutReferences_ProcedureWritten()
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

            var procedure = new Procedure()
            {
                IsDefined = true,
                IsGlobal = true,
                Name = "Proc",
                IsMain = true
            };

            procedure.Code.Add(0xAA);
            using (var stream = new MemoryStream())
            {
                var writer = new AtomWriter();

                var file = new ObjectFile();
                file.Origin = 0x20;
                file.Atoms.Add(procedure);

                writer.Write(file, stream);
                
                CollectionAssert.AreEqual(bytes, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomWriter.Write(ObjectFile, Stream)"/>
        /// can write a procedure with references.
        /// </summary>
        [Test]
        public void Write_ProcedureWithReference_ProcedureRead()
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
                0x01,                           // This is a global reference.
                0x00,                           // The type of reference.
                0x00, 0x00, 0x00, 0x00,         // The address to relocate.
                0x01, 0x00, 0x00, 0x00,         // It is atom number 1 that is being referenced (index based).

                // Null terminated string.
                0x01,                           // Null terminated string type.
                0x01,                           // The string is defined.
                0x00,                           // The string is not global.
                0x53, 0x00,                     // The string is called 'S'.

                // String part
                0x54, 0x78, 0x74, 0x00          // The string is 'Txt'.
            };

            var s = new NullTerminatedString()
            {
                IsDefined = true,
                IsGlobal = false,
                Name = "S",
                Content = "Txt"
            };

            var procedure = new Procedure()
            {
                IsDefined = true,
                IsGlobal = true,
                Name = "Proc",
                IsMain = true
            };

            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.Code.Add(0x00);
            procedure.References.Add(new GlobalReference(s));

            using (var stream = new MemoryStream())
            {
                var writer = new AtomWriter();

                var file = new ObjectFile();
                file.Origin = 0x20;

                file.Atoms.Add(procedure);
                file.Atoms.Add(s);
                
                writer.Write(file, stream);

                CollectionAssert.AreEqual(bytes, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomWriter.Write(ObjectFile, Stream)"/>
        /// can write a null terminated string.
        /// </summary>
        [Test]
        public void Write_NullTerminatedString_StringWritten()
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

            var s = new NullTerminatedString()
            {
                IsDefined = true,
                IsGlobal = true,
                Name = "Txt",
                Content = "Abc"
            };

            using (var stream = new MemoryStream())
            {
                var writer = new AtomWriter();

                var file = new ObjectFile();
                file.Origin = 0x20;
                file.Atoms.Add(s);

                writer.Write(file, stream);

                CollectionAssert.AreEqual(bytes, stream.ToArray());
            }
        }

        /// <summary>
        /// Tests that <see cref="AtomWriter.Write(ObjectFile, Stream)"/>
        /// can write data.
        /// </summary>
        [Test]
        public void Write_Data_DataWritten()
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

            var data = new Data()
            {
                IsDefined = true,
                IsGlobal = true,
                Name = "Data"
            };

            data.Content.Add(0xAA);
            using (var stream = new MemoryStream())
            {
                var writer = new AtomWriter();

                var file = new ObjectFile();
                file.Origin = 0x20;
                file.Atoms.Add(data);

                writer.Write(file, stream);

                CollectionAssert.AreEqual(bytes, stream.ToArray());
            }
        }
    }
}