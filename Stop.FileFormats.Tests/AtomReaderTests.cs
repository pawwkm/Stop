using NUnit.Framework;
using System.IO;

namespace Stop.FileFormats
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
                0x00,                           // The adress is not fixed.
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
                Assert.AreEqual(0x0020, procedure.Address);
                Assert.AreEqual("Proc", procedure.Name);

                Assert.True(procedure.IsMain);
                CollectionAssert.AreEqual(new byte[] { 0xAA }, procedure.Code);
                Assert.AreEqual(0, procedure.References.Count);
            }
        }
    }
}