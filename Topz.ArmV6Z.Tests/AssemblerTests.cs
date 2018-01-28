using NUnit.Framework;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides tests for the <see cref="Assembler"/> class.
    /// </summary>
    public sealed class AssemblerTests
    {
        /// <summary>
        /// Test that <see cref="Assembler.Assemble(string)"/> can assemble a procedure.
        /// </summary>
        /// <param name="source">The code within the procedure.</param>
        /// <param name="expected">The expected assembled code.</param>
        [TestCaseSource(typeof(ProcedureProvider))]
        public void Assemble_Procedure_CodeAssembled(string source, byte[] expected)
        {
            var assembler = new Assembler();
            var obj = assembler.Assemble($@"procedure Main
    {source}");

            Assert.AreEqual(1, obj.Atoms.OfType<FileFormats.Atom.Procedure>().Count());
            var procedure = obj.Atoms.OfType<FileFormats.Atom.Procedure>().First();

            Assert.AreEqual("Main", procedure.Name);
            Assert.AreEqual(expected.ToHex(), procedure.Code.ToHex());
        }
    }
}